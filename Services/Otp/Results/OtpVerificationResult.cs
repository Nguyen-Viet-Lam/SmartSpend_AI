namespace SmartSpendAI.Services.Otp
{
    public sealed class OtpVerificationResult
    {
        public bool Success { get; init; }

        public string Message { get; init; } = string.Empty;
    }
}
