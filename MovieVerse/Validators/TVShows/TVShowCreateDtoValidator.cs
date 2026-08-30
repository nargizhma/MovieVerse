using FluentValidation;
using MovieVerse.Dtos.TVShows;
using MovieVerse.Extensions;

namespace MovieVerse.Validators.TVShows;

public class TVShowCreateDtoValidator
    : AbstractValidator<TVShowCreateDto>
{
    public TVShowCreateDtoValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage(
                "TV show title is required.")
            .MaximumLength(300)
            .WithMessage(
                "TV show title cannot exceed 300 characters.");

        RuleFor(x => x.OriginalTitle)
            .MaximumLength(300)
            .WithMessage(
                "Original title cannot exceed 300 characters.");

        RuleFor(x => x.Synopsis)
            .NotEmpty()
            .WithMessage(
                "TV show synopsis is required.");

        RuleFor(x => x.RuntimeMinutes)
            .GreaterThan(0)
            .WithMessage(
                "Runtime must be greater than 0.")
            .When(x =>
                x.RuntimeMinutes.HasValue);

        RuleFor(x => x.EndDate)
            .Must((dto, endDate) =>
                !endDate.HasValue ||
                endDate.Value >= dto.ReleaseDate)
            .WithMessage(
                "End date cannot be earlier than release date.");

        RuleFor(x => x.PosterImage)
            .Must(file =>
                file!.IsImage())
            .WithMessage(
                "Poster must be an image file.")
            .Must(file =>
                file!.IsValidSize(5))
            .WithMessage(
                "Poster cannot exceed 5 MB.")
            .When(x =>
                x.PosterImage is not null);

        RuleFor(x => x.TrailerUrl)
            .Must(url =>
                Uri.TryCreate(
                    url,
                    UriKind.Absolute,
                    out _))
            .WithMessage(
                "Trailer URL must be valid.")
            .When(x =>
                !string.IsNullOrWhiteSpace(
                    x.TrailerUrl));

        RuleFor(x => x.GenreIds)
            .NotEmpty()
            .WithMessage(
                "TV show must have at least one genre.");

        RuleFor(x => x.GenreIds)
            .Must(ids =>
                ids.Distinct().Count()
                == ids.Count)
            .WithMessage(
                "Genre ids cannot contain duplicates.");

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
                new TVShowActorInputDtoValidator());
    }
}