using Web_Project.Models.Dtos.Finance;

namespace Web_Project.Services.AI
{
    public interface ISmartInputService
    {
        Task<SmartInputResponse> ParseAsync(string input, CancellationToken cancellationToken);
    }
}
