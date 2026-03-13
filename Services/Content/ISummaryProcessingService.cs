using Microsoft.AspNetCore.Http;
using Wed_Project.Models;

namespace Wed_Project.Services.Content
{
    public interface ISummaryProcessingService
    {
        Task<SummarizeUploadResponse> SummarizeUploadAsync(
            IFormFile file,
            CancellationToken cancellationToken);

        Task<SummarizeUploadResponse> SummarizeTextAsync(
            string text,
            string? sourceHint,
            CancellationToken cancellationToken);

        Task<SummarizeUrlResponse> SummarizeFromUrlAsync(
            string url,
            CancellationToken cancellationToken);
    }
}
