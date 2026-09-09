using FluentValidation;
using MovieVerse.Dtos.Episodes;
using MovieVerse.Extensions;

namespace MovieVerse.Validators.Episodes;

public class EpisodeUpdateDtoValidator
    : AbstractValidator<EpisodeUpdateDto>
{
    public EpisodeUpdateDtoValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage(
                "Episode title is required.")
            .MaximumLength(300)
            .WithMessage(
                "Episode title cannot exceed 300 characters.");

        RuleFor(x => x.EpisodeNumber)
            .GreaterThan(0)
            .WithMessage(
                "Episode number must be greater than 0.");

        RuleFor(x => x.RuntimeMinutes)
            .GreaterThan(0)
            .WithMessage(
                "Runtime must be greater than 0.")
            .When(x =>
                x.RuntimeMinutes.HasValue);

        RuleFor(x => x.Image)
            .Must(file =>
                file!.IsImage())
            .WithMessage(
                "Episode image must be an image file.")
            .Must(file =>
                file!.IsValidSize(5))
            .WithMessage(
                "Episode image cannot exceed 5 MB.")
            .When(x =>
                x.Image is not null);

        RuleFor(x => x.Actors)
            .Must(actors =>
                actors
                    .Select(x => x.ActorId)
                    .Distinct()
                    .Count()
                == actors.Count)
            .WithMessage(
                "The same actor cannot be added twice.");

        RuleForEach(x => x.Actors)
            .SetValidator(
                new EpisodeActorInputDtoValidator());

        RuleFor(x => x.DirectorIds)
            .Must(ids =>
                ids.Distinct().Count()
                == ids.Count)
            .WithMessage(
                "Director ids cannot contain duplicates.");

        RuleFor(x => x.WriterIds)
            .Must(ids =>
                ids.Distinct().Count()
                == ids.Count)
            .WithMessage(
                "Writer ids cannot contain duplicates.");
    }
}