namespace LapTrinh_Web.Services.Interfaces;

public interface IReportService
{
    Task<byte[]> ExportTransactionsExcelAsync(Guid userId, DateTime? fromUtc, DateTime? toUtc, CancellationToken cancellationToken = default);
    Task SendWeeklySummaryAsync(Guid userId, CancellationToken cancellationToken = default);
}