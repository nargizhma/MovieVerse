using MovieVerse.Dtos.UserLibrary;

namespace MovieVerse.Services.Interfaces;

public interface IWatchHistoryService
{
    Task<List<LibraryItemReturnDto>> GetMineAsync(
        Guid userId);

    Task MarkMovieWatchedAsync(
        Guid userId,
        Guid movieId);

    Task MarkMovieUnwatchedAsync(
        Guid userId,
        Guid movieId);

    Task MarkTVShowWatchedAsync(
        Guid userId,
        Guid tvShowId);

    Task MarkTVShowUnwatchedAsync(
        Guid userId,
        Guid tvShowId);
}
