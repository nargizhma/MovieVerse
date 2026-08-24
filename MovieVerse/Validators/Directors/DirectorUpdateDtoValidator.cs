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

        RuleFor(x => x)
            .Must(x =>
                !x.BirthDate.HasValue ||
                !x.DeathDate.HasValue ||
                x.DeathDate.Value > x.BirthDate.Value)
            .WithMessage("Death date must be after birth date.");

        RuleFor(x => x.HeightInMeters)
            .GreaterThan(0)
            .WithMessage("Height must be greater than 0.")
            .When(x => x.HeightInMeters.HasValue);
    }
}