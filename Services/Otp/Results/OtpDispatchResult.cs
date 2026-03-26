namespace SmartSpendAI.Services.Otp
{
    public sealed class OtpDispatchResult
    {
        public bool Success { get; init; }

        public string Message { get; init; } = string.Empty;

        public DateTime? ExpiresAt { get; init; }
    }
}
