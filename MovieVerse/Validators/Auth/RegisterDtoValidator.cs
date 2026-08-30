using FluentValidation;
using MovieVerse.Dtos.Auth;

namespace MovieVerse.Validators.Auth;

public class RegisterDtoValidator
    : AbstractValidator<RegisterDto>
{
    public RegisterDtoValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email is required.")
            .EmailAddress()
            .WithMessage("Please enter a valid email address.");

        RuleFor(x => x.UserName)
            .NotEmpty()
            .WithMessage("Username is required.")
            .MinimumLength(3)
            .WithMessage("Username must be at least 3 characters.")
            .MaximumLength(50)
            .WithMessage("Username cannot exceed 50 characters.");

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("Password is required.")
            .MinimumLength(6)
            .WithMessage("Password must be at least 6 characters.")
            .Matches("[A-Z]")
            .WithMessage("Password must contain at least one uppercase letter.")
            .Matches("[a-z]")
            .WithMessage("Password must contain at least one lowercase letter.")
            .Matches("[0-9]")
            .WithMessage("Password must contain at least one digit.")
            .Matches("[^a-zA-Z0-9]")
            .WithMessage("Password must contain at least one special character.");

        RuleFor(x => x.DisplayName)
            .MaximumLength(50)
            .WithMessage("Display name cannot exceed 50 characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.DisplayName));
    }
}