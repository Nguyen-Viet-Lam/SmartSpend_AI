using SmartSpendAI.Models.Dtos.Auth;
using SmartSpendAI.Models.Dtos.Finance;
using SmartSpendAI.Validation.Auth;
using SmartSpendAI.Validation.Finance;

namespace SmartSpendAI.Tests.Validation;

public sealed class RequestValidatorsTests
{
    [Fact]
    public void LoginValidator_ReturnsError_WhenPasswordMissing()
    {
        var validator = new LoginRequestValidator();
        var result = validator.Validate(new LoginRequest
        {
            EmailOrUsername = "demo@smartspend.local",
            Password = string.Empty
        });

        Assert.False(result.IsValid);
    }

    [Fact]
    public void RegisterValidator_ReturnsError_WhenTermsNotAccepted()
    {
        var validator = new RegisterRequestValidator();
        var result = validator.Validate(new RegisterRequest
        {
            Username = "demo.user",
            FullName = "Demo User",
            Email = "demo@smartspend.local",
            Password = "Password123",
            ConfirmPassword = "Password123",
            AcceptTerms = false
        });

        Assert.False(result.IsValid);
    }

    [Fact]
    public void WalletValidator_ReturnsError_WhenTypeUnsupported()
    {
        var validator = new WalletRequestValidator();
        var result = validator.Validate(new WalletRequest
        {
            Name = "Vi test",
            Type = "Crypto",
            InitialBalance = 100_000m
        });

        Assert.False(result.IsValid);
    }

    [Fact]
    public void TransactionValidator_ReturnsError_WhenTypeInvalid()
    {
        var validator = new TransactionRequestValidator();
        var result = validator.Validate(new TransactionRequest
        {
            WalletId = 1,
            CategoryId = 1,
            Type = "Transfer",
            Amount = 100_000m,
            Note = "Test",
            TransactionDate = DateTime.UtcNow
        });

        Assert.False(result.IsValid);
    }

    [Fact]
    public void BudgetValidator_ReturnsError_WhenLimitNotPositive()
    {
        var validator = new BudgetRequestValidator();
        var result = validator.Validate(new BudgetRequest
        {
            CategoryId = 1,
            Month = new DateTime(2026, 3, 1, 0, 0, 0, DateTimeKind.Utc),
            LimitAmount = 0m
        });

        Assert.False(result.IsValid);
    }
}
