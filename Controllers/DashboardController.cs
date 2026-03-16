using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Web_Project.Models;
using Web_Project.Models.Dtos.Dashboard;
using Web_Project.Models.Dtos.Finance;

namespace Web_Project.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class DashboardController : ApiControllerBase
    {
        private readonly AppDbContext _dbContext;

        public DashboardController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public async Task<ActionResult<DashboardResponse>> GetDashboard(CancellationToken cancellationToken)
        {
            var userId = GetCurrentUserId();
            if (userId is null)
            {
                return Unauthorized();
            }

            var monthStart = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1, 0, 0, 0, DateTimeKind.Utc);
            var monthEnd = monthStart.AddMonths(1);
            var previousMonthStart = monthStart.AddMonths(-1);

            var wallets = await _dbContext.Wallets.AsNoTracking().Where(x => x.UserId == userId).ToListAsync(cancellationToken);
            var monthTransactions = await _dbContext.Transactions
                .AsNoTracking()
                .Include(x => x.Category)
                .Where(x => x.UserId == userId && x.TransactionDate >= monthStart && x.TransactionDate < monthEnd)
                .ToListAsync(cancellationToken);

            var previousMonthTransactions = await _dbContext.Transactions
                .AsNoTracking()
                .Include(x => x.Category)
                .Where(x => x.UserId == userId && x.TransactionDate >= previousMonthStart && x.TransactionDate < monthStart)
                .ToListAsync(cancellationToken);

            var budgets = await _dbContext.Budgets
                .AsNoTracking()
                .Include(x => x.Category)
                .Where(x => x.UserId == userId && x.Month == monthStart)
                .ToListAsync(cancellationToken);

            var response = new DashboardResponse
            {
                TotalBalance = wallets.Sum(x => x.Balance),
                TotalIncomeThisMonth = monthTransactions.Where(x => x.Type == "Income").Sum(x => x.Amount),
                TotalExpenseThisMonth = monthTransactions.Where(x => x.Type == "Expense").Sum(x => x.Amount),
                UnreadAlerts = await _dbContext.BudgetAlerts.CountAsync(x => x.UserId == userId && !x.IsRead, cancellationToken),
                MonthlyTrend = await BuildMonthlyTrendAsync(userId.Value, cancellationToken),
                ExpenseBreakdown = monthTransactions
                    .Where(x => x.Type == "Expense")
                    .GroupBy(x => new { x.Category.Name, x.Category.Color })
                    .Select(g => new CategoryBreakdownDto
                    {
                        CategoryName = g.Key.Name,
                        Color = g.Key.Color,
                        Amount = g.Sum(x => x.Amount)
                    })
                    .OrderByDescending(x => x.Amount)
                    .ToList(),
                BudgetProgress = budgets.Select(budget =>
                {
                    var spent = monthTransactions
                        .Where(x => x.Type == "Expense" && x.CategoryId == budget.CategoryId)
                        .Sum(x => x.Amount);
                    var progress = budget.LimitAmount <= 0 ? 0 : decimal.Round(spent / budget.LimitAmount * 100, 2);
                    return new BudgetResponse
                    {
                        BudgetId = budget.BudgetId,
                        CategoryId = budget.CategoryId,
                        CategoryName = budget.Category.Name,
                        CategoryColor = budget.Category.Color,
                        Month = budget.Month,
                        LimitAmount = budget.LimitAmount,
                        SpentAmount = spent,
                        ProgressPercentage = progress,
                        Status = progress >= 100 ? "Danger" : progress >= 80 ? "Warning" : "Safe"
                    };
                }).OrderByDescending(x => x.ProgressPercentage).ToList(),
                Insights = BuildInsights(monthTransactions, previousMonthTransactions),
                Forecasts = BuildForecasts(monthTransactions, budgets)
            };

            return Ok(response);
        }

        private async Task<List<TrendPointDto>> BuildMonthlyTrendAsync(int userId, CancellationToken cancellationToken)
        {
            var points = new List<TrendPointDto>();
            var start = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1, 0, 0, 0, DateTimeKind.Utc).AddMonths(-5);

            for (var i = 0; i < 6; i++)
            {
                var monthStart = start.AddMonths(i);
                var monthEnd = monthStart.AddMonths(1);

                var transactions = await _dbContext.Transactions
                    .AsNoTracking()
                    .Where(x => x.UserId == userId && x.TransactionDate >= monthStart && x.TransactionDate < monthEnd)
                    .ToListAsync(cancellationToken);

                points.Add(new TrendPointDto
                {
                    Label = monthStart.ToString("MM/yyyy"),
                    Income = transactions.Where(x => x.Type == "Income").Sum(x => x.Amount),
                    Expense = transactions.Where(x => x.Type == "Expense").Sum(x => x.Amount)
                });
            }

            return points;
        }

        private static List<string> BuildInsights(List<TransactionEntry> currentMonth, List<TransactionEntry> previousMonth)
        {
            var insights = new List<string>();
            var currentExpense = currentMonth.Where(x => x.Type == "Expense").Sum(x => x.Amount);
            var previousExpense = previousMonth.Where(x => x.Type == "Expense").Sum(x => x.Amount);

            if (previousExpense > 0)
            {
                var change = decimal.Round((currentExpense - previousExpense) / previousExpense * 100, 0);
                if (Math.Abs(change) >= 10)
                {
                    insights.Add($"Tong chi tieu thang nay {(change >= 0 ? "tang" : "giam")} {Math.Abs(change)}% so voi thang truoc.");
                }
            }

            var topIncrease = currentMonth
                .Where(x => x.Type == "Expense")
                .GroupBy(x => x.Category.Name)
                .Select(group =>
                {
                    var currentAmount = group.Sum(x => x.Amount);
                    var previousAmount = previousMonth
                        .Where(x => x.Type == "Expense" && x.Category.Name == group.Key)
                        .Sum(x => x.Amount);
                    return new
                    {
                        CategoryName = group.Key,
                        Delta = currentAmount - previousAmount
                    };
                })
                .OrderByDescending(x => x.Delta)
                .FirstOrDefault();

            if (topIncrease is not null && topIncrease.Delta > 0)
            {
                insights.Add($"Nhom {topIncrease.CategoryName} dang tang manh trong thang nay. Ban nen theo doi sat hon.");
            }

            if (insights.Count == 0)
            {
                insights.Add("Chi tieu cua ban dang on dinh. Hay tiep tuc cap nhat giao dich deu dan de AI dua goi y tot hon.");
            }

            return insights;
        }

        private static List<string> BuildForecasts(List<TransactionEntry> monthTransactions, List<Budget> budgets)
        {
            var forecasts = new List<string>();
            var last7Start = DateTime.UtcNow.Date.AddDays(-6);

            foreach (var budget in budgets)
            {
                var last7Expenses = monthTransactions
                    .Where(x => x.Type == "Expense" && x.CategoryId == budget.CategoryId && x.TransactionDate.Date >= last7Start)
                    .ToList();

                var avgPerDay = last7Expenses.Sum(x => x.Amount) / 7m;
                if (avgPerDay <= 0)
                {
                    continue;
                }

                var spent = monthTransactions
                    .Where(x => x.Type == "Expense" && x.CategoryId == budget.CategoryId)
                    .Sum(x => x.Amount);

                var remaining = budget.LimitAmount - spent;
                if (remaining <= 0)
                {
                    forecasts.Add($"Ngan sach {budget.Category.Name} da het trong thang nay.");
                    continue;
                }

                var daysToDeplete = Math.Floor(remaining / avgPerDay);
                var predictedDate = DateTime.UtcNow.Date.AddDays((double)daysToDeplete);
                if (predictedDate.Month == budget.Month.Month && predictedDate.Year == budget.Month.Year)
                {
                    forecasts.Add($"Voi toc do hien tai, ngan sach {budget.Category.Name} co the can vao ngay {predictedDate:dd/MM}.");
                }
            }

            if (forecasts.Count == 0)
            {
                forecasts.Add("Chua du du lieu de dua du bao can ngan sach.");
            }

            return forecasts;
        }
    }
}
