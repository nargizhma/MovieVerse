using MovieVerse.Dtos.AI;

namespace MovieVerse.Abstractions.AI;

public interface IFilmAiClient
{
    Task<bool> IsFilmRelatedAsync(
        string message,
        IReadOnlyList<FilmChatMessageDto> history,
        CancellationToken cancellationToken = default);

    Task<string> GenerateReplyAsync(
        string message,
        IReadOnlyList<FilmChatMessageDto> history,
        CancellationToken cancellationToken = default);
}