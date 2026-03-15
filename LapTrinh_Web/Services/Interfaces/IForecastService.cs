using LapTrinh_Web.Contracts.Responses.Dashboard;

namespace LapTrinh_Web.Services.Interfaces;

public interface IForecastService
{
    Task<SpendingForecastResponse> PredictEndOfMonthExpenseAsync(Guid userId, int year, int month, CancellationToken cancellationToken = default);
}