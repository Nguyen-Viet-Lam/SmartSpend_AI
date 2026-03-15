namespace LapTrinh_Web.Services.Interfaces;

public interface ICategorySuggestionService
{
    Task<Guid?> SuggestCategoryAsync(Guid userId, string description, CancellationToken cancellationToken = default);
}