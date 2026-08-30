using FluentValidation;
using MovieVerse.Dtos.Profiles;
using MovieVerse.Extensions;

namespace MovieVerse.Validators.Profiles;

public class ProfileUpdateDtoValidator
    : AbstractValidator<ProfileUpdateDto>
{
    public ProfileUpdateDtoValidator()
    {
        RuleFor(x => x.DisplayName)
            .MaximumLength(50)
            .WithMessage(
                "Display name cannot exceed 50 characters.");

        RuleFor(x => x.Bio)
            .MaximumLength(500)
            .WithMessage(
                "Bio cannot exceed 500 characters.");

        RuleFor(x => x.ProfileImage)
            .Must(file =>
                file!.IsImage())
            .WithMessage(
                "Profile image must be an image file.")
            .Must(file =>
                file!.IsValidSize(5))
            .WithMessage(
                "Profile image cannot exceed 5 MB.")
            .When(x =>
                x.ProfileImage is not null);
    }
}
