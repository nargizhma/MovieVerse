using FluentValidation;
using MovieVerse.Dtos.TVShows;

namespace MovieVerse.Validators.TVShows;

public class TVShowActorInputDtoValidator
    : AbstractValidator<TVShowActorInputDto>
{
    public TVShowActorInputDtoValidator()
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