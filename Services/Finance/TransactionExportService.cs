using ClosedXML.Excel;
using Microsoft.EntityFrameworkCore;
using SmartSpendAI.Models;
using SmartSpendAI.Models.Dtos.Finance;

namespace SmartSpendAI.Services.Finance
{
    public class TransactionExportService : ITransactionExportService
    {
        private readonly AppDbContext _dbContext;

        public TransactionExportService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<byte[]> ExportAsync(int userId, TransactionExportFilter filter, CancellationToken cancellationToken)
        {
            var query = _dbContext.Transactions
                .AsNoTracking()
                .Include(x => x.Wallet)
                .Include(x => x.Category)
                .Where(x => x.UserId == userId);

            if (filter.From.HasValue)
            {
                query = query.Where(x => x.TransactionDate >= filter.From.Value.Date);
            }

            if (filter.To.HasValue)
            {
                query = query.Where(x => x.TransactionDate < filter.To.Value.Date.AddDays(1));
            }

            if (filter.WalletId.HasValue)
            {
                query = query.Where(x => x.WalletId == filter.WalletId.Value);
            }

            if (filter.CategoryId.HasValue)
            {
                query = query.Where(x => x.CategoryId == filter.CategoryId.Value);
            }

            if (!string.IsNullOrWhiteSpace(filter.Type))
            {
                query = query.Where(x => x.Type == filter.Type);
            }

            var transactions = await query
                .OrderByDescending(x => x.TransactionDate)
                .ThenByDescending(x => x.TransactionEntryId)
                .Select(x => new
                {
                    x.TransactionDate,
                    x.Note,
                    WalletName = x.Wallet.Name,
                    CategoryName = x.Category.Name,
                    x.Type,
                    x.Amount,
                    x.AiConfidence,
                    x.CreatedAt
                })
                .ToListAsync(cancellationToken);

            using var workbook = new XLWorkbook();
            var sheet = workbook.Worksheets.Add("Transactions");

            var headers = new[]
            {
                "TransactionDate",
                "Note",
                "Wallet",
                "Category",
                "Type",
                "Amount",
                "AiConfidence",
                "CreatedAtUtc"
            };

            for (var i = 0; i < headers.Length; i++)
            {
                sheet.Cell(1, i + 1).Value = headers[i];
                sheet.Cell(1, i + 1).Style.Font.Bold = true;
            }

            for (var rowIndex = 0; rowIndex < transactions.Count; rowIndex++)
            {
                var row = rowIndex + 2;
                var item = transactions[rowIndex];
                sheet.Cell(row, 1).Value = item.TransactionDate;
                sheet.Cell(row, 2).Value = item.Note;
                sheet.Cell(row, 3).Value = item.WalletName;
                sheet.Cell(row, 4).Value = item.CategoryName;
                sheet.Cell(row, 5).Value = item.Type;
                sheet.Cell(row, 6).Value = item.Amount;
                sheet.Cell(row, 7).Value = item.AiConfidence;
                sheet.Cell(row, 8).Value = item.CreatedAt;
            }

            sheet.Column(1).Style.DateFormat.Format = "yyyy-mm-dd";
            sheet.Column(6).Style.NumberFormat.Format = "#,##0.00";
            sheet.Column(7).Style.NumberFormat.Format = "0.00";
            sheet.Column(8).Style.DateFormat.Format = "yyyy-mm-dd hh:mm:ss";
            sheet.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }
    }
}
