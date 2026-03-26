using FluentValidation;
using SmartSpendAI.Models.Dtos.Finance;

namespace SmartSpendAI.Validation.Finance
{
    public class WalletRequestValidator : AbstractValidator<WalletRequest>
    {
        private static readonly string[] AllowedWalletTypes = ["Cash", "Bank", "Savings", "Momo"];

        public WalletRequestValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .MaximumLength(100);

            RuleFor(x => x.Type)
                .NotEmpty()
                .MaximumLength(32)
                .Must(type => AllowedWalletTypes.Contains(type))
                .WithMessage("Wallet type must be one of: Cash, Bank, Savings, Momo.");

            RuleFor(x => x.InitialBalance)
                .GreaterThanOrEqualTo(0m)
                .LessThanOrEqualTo(999_999_999m);
        }
    }
}
