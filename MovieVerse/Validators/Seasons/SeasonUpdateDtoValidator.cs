using FluentValidation;
using MovieVerse.Dtos.Seasons;

namespace MovieVerse.Validators.Seasons;

public class SeasonUpdateDtoValidator
    : AbstractValidator<SeasonUpdateDto>
{
    public SeasonUpdateDtoValidator()
    {
        RuleFor(x => x.SeasonNumber)
            .GreaterThan(0)
            .WithMessage(
                "Season number must be greater than 0.");
    }
}