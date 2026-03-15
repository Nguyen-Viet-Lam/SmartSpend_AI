using LapTrinh_Web.Contracts.Responses.Dashboard;
using LapTrinh_Web.Services.Interfaces;

namespace LapTrinh_Web.Services.Implementations;

public sealed class ForecastService : IForecastService
{
    public Task<SpendingForecastResponse> PredictEndOfMonthExpenseAsync(Guid userId, int year, int month, CancellationToken cancellationToken = default)
        => throw new NotImplementedException();
}