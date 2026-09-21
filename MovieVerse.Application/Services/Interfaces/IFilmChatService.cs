using MovieVerse.Dtos.AI;

namespace MovieVerse.Services.Interfaces;

public interface IFilmChatService
{
    Task<FilmChatResponseDto> SendAsync(
        FilmChatRequestDto request,
        CancellationToken cancellationToken = default);
}