using SmartSpendAI.Models.Dtos.Auth;

namespace SmartSpendAI.Services.Auth
{
    public sealed class LoginServiceResult
    {
        public bool Success { get; init; }

        public int StatusCode { get; init; } = 400;

        public string Message { get; init; } = string.Empty;

        public LoginResponse? Response { get; init; }

        public Dictionary<string, string[]> ValidationErrors { get; init; } = new();
    }
}
