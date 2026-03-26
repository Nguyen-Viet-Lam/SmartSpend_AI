using SmartSpendAI.Models.Dtos.Auth;

namespace SmartSpendAI.Services.Auth
{
    public sealed class RegisterServiceResult
    {
        public bool Success { get; init; }

        public bool IsConflict { get; init; }

        public string Message { get; init; } = string.Empty;

        public RegisterResponse? Response { get; init; }

        public Dictionary<string, string[]> ValidationErrors { get; init; } = new();
    }
}
