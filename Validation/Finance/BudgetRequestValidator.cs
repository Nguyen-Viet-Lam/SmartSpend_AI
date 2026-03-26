using FluentValidation;
using SmartSpendAI.Models.Dtos.Finance;

namespace SmartSpendAI.Validation.Finance
{
    public class BudgetRequestValidator : AbstractValidator<BudgetRequest>
    {
        public BudgetRequestValidator()
        {
            RuleFor(x => x.CategoryId)
                .GreaterThan(0);

            RuleFor(x => x.Month)
                .GreaterThan(new DateTime(2000, 1, 1));

            RuleFor(x => x.LimitAmount)
                .GreaterThan(0m)
                .LessThanOrEqualTo(999_999_999m);
        }
    }
}
