using FluentValidation;
using MovieVerse.Dtos.Genres;

namespace MovieVerse.Validators.Genres;

public class GenreCreateDtoValidator
    : AbstractValidator<GenreCreateDto>
{
    public GenreCreateDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Genre name is required.")
            .MaximumLength(100)
            .WithMessage("Genre name cannot exceed 100 characters.");
    }
}