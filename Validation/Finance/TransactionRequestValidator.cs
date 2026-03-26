using FluentValidation;
using SmartSpendAI.Models.Dtos.Finance;

namespace SmartSpendAI.Validation.Finance
{
    public class TransactionRequestValidator : AbstractValidator<TransactionRequest>
    {
        private static readonly string[] AllowedTypes = ["Expense", "Income"];

        public TransactionRequestValidator()
        {
            RuleFor(x => x.WalletId)
                .GreaterThan(0);

            RuleFor(x => x.CategoryId)
                .GreaterThan(0);

            RuleFor(x => x.Type)
                .NotEmpty()
                .MaximumLength(32)
                .Must(type => AllowedTypes.Contains(type))
                .WithMessage("Type must be Expense or Income.");

            RuleFor(x => x.Amount)
                .GreaterThan(0m)
                .LessThanOrEqualTo(999_999_999m);

            RuleFor(x => x.Note)
                .MaximumLength(400);

            RuleFor(x => x.TransactionDate)
                .GreaterThan(new DateTime(2000, 1, 1));
        }
    }
}
