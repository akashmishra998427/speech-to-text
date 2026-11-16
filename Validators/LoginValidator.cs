using FluentValidation;
using speect_to_text_Api.DTO;

namespace speect_to_text_Api.Validators
{
    public class LoginValidator : AbstractValidator<Login>
    {
        public LoginValidator()
        {
            RuleFor(x => x.email)
                .NotEmpty()
                .WithMessage("Email is required.");

            RuleFor(x => x.password)
                .NotEmpty()
                .WithMessage("Password is required.");
        }
    }
}
