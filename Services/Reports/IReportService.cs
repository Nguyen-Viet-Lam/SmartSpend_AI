using SmartSpendAI.Models.Dtos.Reports;

namespace SmartSpendAI.Services.Reports
{
    public interface IReportService
    {
        Task<ReportPeriodSummaryResponse> GetWeeklySummaryAsync(int userId, CancellationToken cancellationToken);

        Task<ReportPeriodSummaryResponse> GetMonthlySummaryAsync(int userId, DateTime? month, CancellationToken cancellationToken);

        Task<IReadOnlyList<ReportEmailHistoryResponse>> GetEmailHistoryAsync(int userId, int take, CancellationToken cancellationToken);
    }
}
