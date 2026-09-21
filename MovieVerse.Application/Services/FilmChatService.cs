using MovieVerse.Abstractions.AI;
using MovieVerse.Dtos.AI;
using MovieVerse.Services.Interfaces;

namespace MovieVerse.Services;

public class FilmChatService(
    IFilmAiClient filmAiClient)
    : IFilmChatService
{
    private const string OffTopicReply =
        "I can only help with movies, TV shows, anime, cinema, and film recommendations.";

    public async Task<FilmChatResponseDto> SendAsync(
        FilmChatRequestDto request,
        CancellationToken cancellationToken = default)
    {
        var history = request.History
            .TakeLast(12)
            .ToList();

        var isFilmRelated =
            await filmAiClient.IsFilmRelatedAsync(
                request.Message,
                history,
                cancellationToken);

        if (!isFilmRelated)
        {
            return new FilmChatResponseDto
            {
                Reply = OffTopicReply,
                IsFilmRelated = false
            };
        }

        var reply =
            await filmAiClient.GenerateReplyAsync(
                request.Message,
                history,
                cancellationToken);

        return new FilmChatResponseDto
        {
            Reply = reply,
            IsFilmRelated = true
        };
    }
}