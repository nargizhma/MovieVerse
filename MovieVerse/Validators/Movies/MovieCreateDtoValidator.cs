using FluentValidation;
using MovieVerse.Dtos.Movies;
using MovieVerse.Extensions;

namespace MovieVerse.Validators.Movies;

public class MovieCreateDtoValidator
    : AbstractValidator<MovieCreateDto>
{
    public MovieCreateDtoValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage(
                "Movie title is required.")
            .MaximumLength(300)
            .WithMessage(
                "Movie title cannot exceed 300 characters.");

        RuleFor(x => x.Synopsis)
            .NotEmpty()
            .WithMessage(
                "Movie synopsis is required.");

        RuleFor(x => x.RuntimeMinutes)
            .GreaterThan(0)
            .WithMessage(
                "Runtime must be greater than 0.");

        RuleFor(x => x.PosterImage)
            .Must(file => file!.IsImage())
            .WithMessage(
                "Poster must be an image file.")
            .Must(file => file!.IsValidSize(5))
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

        RuleFor(x => x.Budget)
            .GreaterThanOrEqualTo(0)
            .WithMessage(
                "Budget cannot be negative.")
            .When(x =>
                x.Budget.HasValue);

        RuleFor(x => x.GrossWorldwide)
            .GreaterThanOrEqualTo(0)
            .WithMessage(
                "Gross worldwide cannot be negative.")
            .When(x =>
                x.GrossWorldwide.HasValue);

        RuleFor(x => x.GenreIds)
            .NotEmpty()
            .WithMessage(
                "Movie must have at least one genre.");

        RuleFor(x => x.GenreIds)
            .Must(ids =>
                ids.Distinct().Count()
                == ids.Count)
            .WithMessage(
                "Genre ids cannot contain duplicates.");

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
                new MovieActorInputDtoValidator());
    }
}