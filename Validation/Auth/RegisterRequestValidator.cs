using FluentValidation;
using SmartSpendAI.Models.Dtos.Auth;

namespace SmartSpendAI.Validation.Auth
{
    public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
    {
        public RegisterRequestValidator()
        {
            RuleFor(x => x.Username)
                .NotEmpty()
                .Length(3, 64)
                .Matches("^[a-zA-Z0-9._-]{3,64}$");

            RuleFor(x => x.FullName)
                .NotEmpty()
                .MaximumLength(128);

            RuleFor(x => x.Email)
                .NotEmpty()
                .EmailAddress()
                .MaximumLength(256);

            RuleFor(x => x.Password)
                .NotEmpty()
                .Length(8, 128);

            RuleFor(x => x.ConfirmPassword)
                .NotEmpty()
                .Equal(x => x.Password);

            RuleFor(x => x.AcceptTerms)
                .Equal(true);
        }
    }
}
