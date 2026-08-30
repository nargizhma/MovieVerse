using FluentValidation;
using MovieVerse.Dtos.Episodes;

namespace MovieVerse.Validators.Episodes;

public class EpisodeActorInputDtoValidator
    : AbstractValidator<EpisodeActorInputDto>
{
    public EpisodeActorInputDtoValidator()
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