using MovieVerse.Dtos.UserLibrary;

namespace MovieVerse.Services.Interfaces;

public interface IWatchlistService
{
    Task<List<LibraryItemReturnDto>> GetMineAsync(
        Guid userId);

    Task AddMovieAsync(
        Guid userId,
        Guid movieId);

    Task RemoveMovieAsync(
        Guid userId,
        Guid movieId);

    Task AddTVShowAsync(
        Guid userId,
        Guid tvShowId);

    Task RemoveTVShowAsync(
        Guid userId,
        Guid tvShowId);
}
