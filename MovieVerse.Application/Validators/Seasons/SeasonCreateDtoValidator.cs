using FluentValidation;
using MovieVerse.Dtos.Seasons;

namespace MovieVerse.Validators.Seasons;

public class SeasonCreateDtoValidator
    : AbstractValidator<SeasonCreateDto>
{
    public SeasonCreateDtoValidator()
    {
        RuleFor(x => x.SeasonNumber)
            .GreaterThan(0)
            .WithMessage(
                "Season number must be greater than 0.");
    }
}