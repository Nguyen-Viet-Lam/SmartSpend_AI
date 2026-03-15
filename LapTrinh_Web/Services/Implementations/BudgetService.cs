using LapTrinh_Web.Contracts.Requests.Budgets;
using LapTrinh_Web.Contracts.Responses.Budgets;
using LapTrinh_Web.Services.Interfaces;

namespace LapTrinh_Web.Services.Implementations;

public sealed class BudgetService : IBudgetService
{
    public Task<IReadOnlyList<BudgetProgressResponse>> GetMonthlyProgressAsync(Guid userId, int year, int month, CancellationToken cancellationToken = default)
        => throw new NotImplementedException();

    public Task<BudgetProgressResponse> CreateAsync(Guid userId, CreateBudgetRequest request, CancellationToken cancellationToken = default)
        => throw new NotImplementedException();

    public Task<BudgetProgressResponse> UpdateAsync(Guid userId, Guid budgetId, UpdateBudgetRequest request, CancellationToken cancellationToken = default)
        => throw new NotImplementedException();
}