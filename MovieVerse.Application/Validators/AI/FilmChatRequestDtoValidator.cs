using FluentValidation;
using MovieVerse.Dtos.AI;

namespace MovieVerse.Validators.AI;

public class FilmChatRequestDtoValidator
    : AbstractValidator<FilmChatRequestDto>
{
    public FilmChatRequestDtoValidator()
    {
        RuleFor(x => x.Message)
            .NotEmpty()
            .WithMessage("Message is required.")
            .MaximumLength(500)
            .WithMessage(
                "Message cannot exceed 500 characters.");

        RuleFor(x => x.History)
            .NotNull()
            .Must(x => x.Count <= 12)
            .WithMessage(
                "Chat history cannot contain more than 12 messages.");

        RuleForEach(x => x.History)
            .ChildRules(message =>
            {
                message.RuleFor(x => x.Role)
                    .Must(role =>
                        string.Equals(
                            role,
                            "user",
                            StringComparison.OrdinalIgnoreCase)
                        ||
                        string.Equals(
                            role,
                            "model",
                            StringComparison.OrdinalIgnoreCase))
                    .WithMessage(
                        "Chat message role must be user or model.");

                message.RuleFor(x => x.Text)
                    .NotEmpty()
                    .MaximumLength(1500);
            });
    }
}