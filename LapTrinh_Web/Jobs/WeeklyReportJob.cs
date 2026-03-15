using LapTrinh_Web.Services.Interfaces;

namespace LapTrinh_Web.Jobs;

public sealed class WeeklyReportJob(IReportService reportService)
{
    public Task ExecuteAsync(CancellationToken cancellationToken = default)
    {
        _ = reportService;
        _ = cancellationToken;
        return Task.CompletedTask;
    }
}