using FluentValidation;
using speect_to_text_Api.DTO;

namespace speect_to_text_Api.Validators
{
    public class UserRegValidator : AbstractValidator<UserReg>
    {
        public UserRegValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("Name is required.")
                .MaximumLength(50)
                .WithMessage("Name cannot exceed 50 characters.");

            RuleFor(x => x.Phone)
                .NotEmpty()
                .WithMessage("Phone number is required.")
                .Matches(@"^\+?[0-9\s-]{10,20}$")
                .WithMessage("Enter a valid phone number.");

            RuleFor(x => x.Email)
                .NotEmpty()
                .WithMessage("Email is required.")
                .EmailAddress()
                .WithMessage("Enter a valid email address.")
                .MaximumLength(255);

            RuleFor(x => x.Gender)
                .NotEmpty()
                .WithMessage("Please select your gender.")
                .Must(x =>
                    x == "Male" ||
                    x == "Female" ||
                    x == "Other")
                .WithMessage("Invalid gender selected.");

            RuleFor(x => x.Profession)
                .NotEmpty()
                .WithMessage("Please select your profession.");

            RuleFor(x => x.Address)
                .NotEmpty()
                .WithMessage("Address is required.")
                .MaximumLength(250);

            RuleFor(x => x.Password)
                .NotEmpty()
                .WithMessage("Password is required.")
                .MinimumLength(8)
                .WithMessage("Password must be at least 8 characters.")
                .Matches("[A-Z]")
                .WithMessage("Password must contain an uppercase letter.")
                .Matches("[a-z]")
                .WithMessage("Password must contain a lowercase letter.")
                .Matches("[0-9]")
                .WithMessage("Password must contain a number.")
                .Matches(@"[^a-zA-Z0-9]")
                .WithMessage("Password must contain a special character.");

            RuleFor(x => x.ConfirmPassword)
                .NotEmpty()
                .WithMessage("Please confirm your password.")
                .Equal(x => x.Password)
                .WithMessage("Passwords do not match.");
        }
    }
}