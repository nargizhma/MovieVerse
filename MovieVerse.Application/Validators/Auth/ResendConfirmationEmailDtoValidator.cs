using FluentValidation;
using MovieVerse.Dtos.Auth;

namespace MovieVerse.Validators.Auth;

public class ResendConfirmationEmailDtoValidator
    : AbstractValidator<
        ResendConfirmationEmailDto>
{
    public ResendConfirmationEmailDtoValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email is required.")
            .EmailAddress()
            .WithMessage(
                "Please enter a valid email address.");
    }
}