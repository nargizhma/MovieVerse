using FluentValidation;
using MovieVerse.Dtos.Actors;
using MovieVerse.Extensions;

namespace MovieVerse.Validators.Actors;

public class ActorCreateDtoValidator
    : AbstractValidator<ActorCreateDto>
{
    public ActorCreateDtoValidator()
    {
        RuleFor(x => x.FullName)
            .NotEmpty()
            .WithMessage("Actor name is required.")
            .MaximumLength(200)
            .WithMessage("Actor name cannot exceed 200 characters.");

        RuleFor(x => x.ProfileImage)
            .Must(file => file!.IsImage())
            .WithMessage("Profile image must be an image file.")
            .Must(file => file!.IsValidSize(5))
            .WithMessage("Profile image cannot exceed 5 MB.")
            .When(x => x.ProfileImage is not null);

        RuleFor(x => x.HeightInMeters)
            .GreaterThan(0)
            .WithMessage("Height must be greater than 0.")
            .When(x => x.HeightInMeters.HasValue);

        RuleFor(x => x.BirthDate)
            .LessThan(DateTime.UtcNow)
            .WithMessage("Birth date cannot be in the future.")
            .When(x => x.BirthDate.HasValue);
    }
}