using FluentValidation;
using MovieVerse.Dtos.Reviews;

namespace MovieVerse.Validators.Reviews;

public class ReviewCreateDtoValidator
    : AbstractValidator<ReviewCreateDto>
{
    public ReviewCreateDtoValidator()
    {
        RuleFor(x => x.Rating)
            .InclusiveBetween(1m, 10m)
            .WithMessage(
                "Rating must be between 1 and 10.")
            .Must(rating =>
                decimal.Round(rating, 1) == rating)
            .WithMessage(
                "Rating can have at most one decimal place.");

        RuleFor(x => x.Content)
            .MaximumLength(5000)
            .WithMessage(
                "Review cannot exceed 5000 characters.");
    }
}
