using FluentValidation;
using MovieVerse.Dtos.Movies;

namespace MovieVerse.Validators.Movies;

public class MovieActorInputDtoValidator
    : AbstractValidator<MovieActorInputDto>
{
    public MovieActorInputDtoValidator()
    {
        RuleFor(x => x.ActorId)
            .NotEmpty()
            .WithMessage(
                "Actor id is required.");

        RuleFor(x => x.CharacterName)
            .MaximumLength(200)
            .WithMessage(
                "Character name cannot exceed 200 characters.");

        RuleFor(x => x.CastOrder)
            .GreaterThanOrEqualTo(0)
            .WithMessage(
                "Cast order cannot be negative.");
    }
}