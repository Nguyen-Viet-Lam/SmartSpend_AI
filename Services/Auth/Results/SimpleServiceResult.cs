namespace SmartSpendAI.Services.Auth
{
    public sealed class SimpleServiceResult
    {
        public bool Success { get; init; }

        public string Message { get; init; } = string.Empty;

        public Dictionary<string, string[]> ValidationErrors { get; init; } = new();
    }
}
