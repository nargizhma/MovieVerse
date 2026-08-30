using FluentValidation;
using MovieVerse.Dtos.Recommendations;

namespace MovieVerse.Validators.Recommendations;

public class SimilarTVShowsQueryDtoValidator
    : AbstractValidator<SimilarTVShowsQueryDto>
{
    public SimilarTVShowsQueryDtoValidator()
    {
        RuleFor(x => x.Limit)
            .InclusiveBetween(
                1,
                20)
            .WithMessage(
                "Limit must be between 1 and 20.");
    }
}