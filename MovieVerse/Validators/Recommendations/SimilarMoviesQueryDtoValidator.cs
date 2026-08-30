using FluentValidation;
using MovieVerse.Dtos.Recommendations;

namespace MovieVerse.Validators.Recommendations;

public class SimilarMoviesQueryDtoValidator
    : AbstractValidator<SimilarMoviesQueryDto>
{
    public SimilarMoviesQueryDtoValidator()
    {
        RuleFor(x => x.Limit)
            .InclusiveBetween(1, 20)
            .WithMessage(
                "Limit must be between 1 and 20.");
    }
}
