using LapTrinh_Web.Services.Interfaces;

namespace LapTrinh_Web.Services.Implementations;

public sealed class ReportService : IReportService
{
    public Task<byte[]> ExportTransactionsExcelAsync(Guid userId, DateTime? fromUtc, DateTime? toUtc, CancellationToken cancellationToken = default)
        => throw new NotImplementedException();

    public Task SendWeeklySummaryAsync(Guid userId, CancellationToken cancellationToken = default)
        => throw new NotImplementedException();
}