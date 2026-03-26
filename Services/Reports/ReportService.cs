using Microsoft.EntityFrameworkCore;
using SmartSpendAI.Models;
using SmartSpendAI.Models.Dtos.Reports;

namespace SmartSpendAI.Services.Reports
{
    public class ReportService : IReportService
    {
        private static readonly HashSet<string> EmailHistoryActions = new(StringComparer.OrdinalIgnoreCase)
        {
            "WeeklySummarySent",
            "UserLoginNewDeviceAlertSent"
        };

        private readonly AppDbContext _dbContext;

        public ReportService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Task<ReportPeriodSummaryResponse> GetWeeklySummaryAsync(int userId, CancellationToken cancellationToken)
        {
            var endExclusive = DateTime.UtcNow.Date.AddDays(1);
            var startInclusive = endExclusive.AddDays(-7);
            return BuildPeriodSummaryAsync(userId, startInclusive, endExclusive, "7 ngày gần nhất", cancellationToken);
        }

        public Task<ReportPeriodSummaryResponse> GetMonthlySummaryAsync(int userId, DateTime? month, CancellationToken cancellationToken)
        {
            var source = month ?? DateTime.UtcNow;
            var startInclusive = new DateTime(source.Year, source.Month, 1, 0, 0, 0, DateTimeKind.Utc);
            var endExclusive = startInclusive.AddMonths(1);
            var periodLabel = $"Tháng {source:MM/yyyy}";
            return BuildPeriodSummaryAsync(userId, startInclusive, endExclusive, periodLabel, cancellationToken);
        }

        public async Task<IReadOnlyList<ReportEmailHistoryResponse>> GetEmailHistoryAsync(
            int userId,
            int take,
            CancellationToken cancellationToken)
        {
            var safeTake = Math.Clamp(take, 1, 100);
            var recipientEmail = await _dbContext.Users
                .AsNoTracking()
                .Where(x => x.UserId == userId)
                .Select(x => x.Email)
                .FirstOrDefaultAsync(cancellationToken) ?? string.Empty;

            var logs = await _dbContext.AuditLogs
                .AsNoTracking()
                .Where(x => x.ActorUserId == userId && EmailHistoryActions.Contains(x.Action))
                .OrderByDescending(x => x.CreatedAt)
                .Take(safeTake)
                .Select(x => new ReportEmailHistoryResponse
                {
                    AuditLogId = x.AuditLogId,
                    EventType = MapEventType(x.Action),
                    RecipientEmail = recipientEmail,
                    Subject = BuildSubject(x.Action, x.Metadata),
                    Metadata = string.IsNullOrWhiteSpace(x.Metadata) ? "--" : x.Metadata,
                    SentAt = x.CreatedAt
                })
                .ToListAsync(cancellationToken);

            return logs;
        }

        private async Task<ReportPeriodSummaryResponse> BuildPeriodSummaryAsync(
            int userId,
            DateTime startInclusive,
            DateTime endExclusive,
            string periodLabel,
            CancellationToken cancellationToken)
        {
            var transactions = await _dbContext.Transactions
                .AsNoTracking()
                .Include(x => x.Category)
                .Where(x => x.UserId == userId &&
                            x.TransactionDate >= startInclusive &&
                            x.TransactionDate < endExclusive)
                .ToListAsync(cancellationToken);

            var totalIncome = transactions
                .Where(x => string.Equals(x.Type, "Income", StringComparison.OrdinalIgnoreCase))
                .Sum(x => x.Amount);

            var totalExpense = transactions
                .Where(x => string.Equals(x.Type, "Expense", StringComparison.OrdinalIgnoreCase))
                .Sum(x => x.Amount);

            var topExpenseCategories = transactions
                .Where(x => string.Equals(x.Type, "Expense", StringComparison.OrdinalIgnoreCase))
                .GroupBy(x => new { x.Category.Name, x.Category.Color })
                .Select(group =>
                {
                    var amount = group.Sum(x => x.Amount);
                    return new ReportCategorySummaryResponse
                    {
                        CategoryName = group.Key.Name,
                        Color = group.Key.Color,
                        Amount = amount,
                        Percentage = totalExpense > 0
                            ? decimal.Round(amount * 100m / totalExpense, 2)
                            : 0
                    };
                })
                .OrderByDescending(x => x.Amount)
                .Take(5)
                .ToList();

            return new ReportPeriodSummaryResponse
            {
                PeriodLabel = periodLabel,
                StartDate = startInclusive.Date,
                EndDate = endExclusive.Date.AddDays(-1),
                TotalIncome = totalIncome,
                TotalExpense = totalExpense,
                NetAmount = totalIncome - totalExpense,
                TransactionCount = transactions.Count,
                TopExpenseCategories = topExpenseCategories
            };
        }

        private static string MapEventType(string action)
        {
            if (string.Equals(action, "WeeklySummarySent", StringComparison.OrdinalIgnoreCase))
            {
                return "Weekly Summary";
            }

            if (string.Equals(action, "UserLoginNewDeviceAlertSent", StringComparison.OrdinalIgnoreCase))
            {
                return "Security Alert";
            }

            return action;
        }

        private static string BuildSubject(string action, string metadata)
        {
            if (string.Equals(action, "WeeklySummarySent", StringComparison.OrdinalIgnoreCase))
            {
                return $"Tóm tắt tuần {metadata}";
            }

            if (string.Equals(action, "UserLoginNewDeviceAlertSent", StringComparison.OrdinalIgnoreCase))
            {
                return "Cảnh báo bảo mật đăng nhập thiết bị lạ";
            }

            return action;
        }
    }
}
