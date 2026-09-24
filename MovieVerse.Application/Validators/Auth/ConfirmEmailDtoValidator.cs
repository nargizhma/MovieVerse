using FluentValidation;
using MovieVerse.Dtos.Auth;

namespace MovieVerse.Validators.Auth;

public class ConfirmEmailDtoValidator
    : AbstractValidator<ConfirmEmailDto>
{
    public ConfirmEmailDtoValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage(
                "User ID is required.");

        RuleFor(x => x.Token)
            .NotEmpty()
            .WithMessage(
                "Verification token is required.");
    }
}