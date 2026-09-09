using FluentValidation;
using MovieVerse.Dtos.Common;

namespace MovieVerse.Validators.Common;

public class CatalogFilterDtoValidator
    : AbstractValidator<CatalogFilterDto>
{
    private static readonly string[] AllowedSortFields =
    [
        "title",
        "year",
        "rating"
    ];

    public CatalogFilterDtoValidator()
    {
        RuleFor(x => x.Search)
            .MaximumLength(200)
            .WithMessage(
                "Search text cannot exceed 200 characters.")
            .When(x =>
                !string.IsNullOrWhiteSpace(x.Search));

        RuleFor(x => x.ReleaseYear)
            .InclusiveBetween(1888, 2100)
            .WithMessage(
                "Release year must be between 1888 and 2100.")
            .When(x =>
                x.ReleaseYear.HasValue);

        RuleFor(x => x.MinRating)
            .InclusiveBetween(1m, 10m)
            .WithMessage(
                "Minimum rating must be between 1 and 10.")
            .When(x =>
                x.MinRating.HasValue);

        RuleFor(x => x.MaxRating)
            .InclusiveBetween(1m, 10m)
            .WithMessage(
                "Maximum rating must be between 1 and 10.")
            .When(x =>
                x.MaxRating.HasValue);

        RuleFor(x => x)
            .Must(x =>
                !x.MinRating.HasValue ||
                !x.MaxRating.HasValue ||
                x.MinRating.Value <=
                x.MaxRating.Value)
            .WithMessage(
                "Minimum rating cannot be greater than maximum rating.");

        RuleFor(x => x.SortBy)
            .Must(sortBy =>
                AllowedSortFields.Contains(
                    sortBy!.Trim().ToLowerInvariant()))
            .WithMessage(
                "SortBy must be title, year, or rating.")
            .When(x =>
                !string.IsNullOrWhiteSpace(x.SortBy));

        RuleFor(x => x.PageNumber)
            .GreaterThan(0)
            .WithMessage(
                "Page number must be greater than 0.");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100)
            .WithMessage(
                "Page size must be between 1 and 100.");
    }
}
