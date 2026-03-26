using SmartSpendAI.Models.Dtos.Finance;

namespace SmartSpendAI.Services.AI
{
    public interface ISmartInputService
    {
        Task<SmartInputResponse> ParseAsync(string input, int userId, CancellationToken cancellationToken);

        Task LearnFromCorrectionAsync(string input, int userId, int correctedCategoryId, CancellationToken cancellationToken);
    }
}
