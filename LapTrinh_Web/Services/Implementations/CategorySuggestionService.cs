using LapTrinh_Web.Services.Interfaces;

namespace LapTrinh_Web.Services.Implementations;

public sealed class CategorySuggestionService : ICategorySuggestionService
{
    public Task<Guid?> SuggestCategoryAsync(Guid userId, string description, CancellationToken cancellationToken = default)
        => throw new NotImplementedException();
}