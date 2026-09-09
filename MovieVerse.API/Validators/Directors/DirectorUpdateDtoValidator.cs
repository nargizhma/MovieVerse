using FluentValidation;
using MovieVerse.Dtos.Directors;
using MovieVerse.Extensions;

namespace MovieVerse.Validators.Directors;

public class DirectorUpdateDtoValidator
    : AbstractValidator<DirectorUpdateDto>
{
    public DirectorUpdateDtoValidator()
    {
        RuleFor(x => x.FullName)
            .NotEmpty()
            .WithMessage("Director name is required.")
            .MaximumLength(200)
            .WithMessage("Director name cannot exceed 200 characters.");

        RuleFor(x => x.ProfileImage)
            .Must(file => file!.IsImage())
            .WithMessage("Profile image must be an image file.")
            .Must(file => file!.IsValidSize(5))
            .WithMessage("Profile image cannot exceed 5 MB.")
            .When(x => x.ProfileImage is not null);

        RuleFor(x => x.BirthDate)
            .LessThan(DateTime.UtcNow)
            .WithMessage("Birth date cannot be in the future.")
            .When(x => x.BirthDate.HasValue);

        RuleFor(x => x.DeathDate)
            .LessThanOrEqualTo(DateTime.UtcNow)
            .WithMessage("Death date cannot be in the future.")
            .When(x => x.DeathDate.HasValue);

        RuleFor(x => x.DeathDate)
            .GreaterThan(x => x.BirthDate)
            .WithMessage(
                "Death date must be after birth date.")
            .When(x =>
                x.DeathDate.HasValue &&
                x.BirthDate.HasValue);

        RuleFor(x => x.HeightInMeters)
            .GreaterThan(0)
            .WithMessage("Height must be greater than 0.")
            .When(x => x.HeightInMeters.HasValue);
        RuleFor(x => x.BirthPlace)
            .MaximumLength(200)
            .WithMessage(
                "Birth place cannot exceed 200 characters.");

        RuleFor(x => x.DeathPlace)
            .MaximumLength(200)
            .WithMessage(
                "Death place cannot exceed 200 characters.");

        RuleFor(x => x.AlternativeName)
            .MaximumLength(200)
            .WithMessage(
                "Alternative name cannot exceed 200 characters.");
    }
}