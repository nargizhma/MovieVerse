using MovieVerse.Abstractions.Media;
using MovieVerse.Dtos.UserLibrary;
using MovieVerse.Exceptions;
using MovieVerse.Models;
using MovieVerse.Repositories.Interfaces;
using MovieVerse.Services.Interfaces;

namespace MovieVerse.Services;

public class WatchHistoryService(
    IGenericRepository<WatchHistoryItem> watchHistoryRepository,
    IGenericRepository<Movie> movieRepository,
    IGenericRepository<TVShow> tvShowRepository,
    IMediaUrlBuilder mediaUrlBuilder)
    : IWatchHistoryService
{
    public async Task<List<LibraryItemReturnDto>> GetMineAsync(
        Guid userId)
    {
        var items =
            await watchHistoryRepository.FindAllAsync(
                x => x.UserId == userId,
                false,
                "Movie.Reviews",
                "TVShow.Reviews");

        return items
            .OrderByDescending(x => x.WatchedAt)
            .Select(MapItem)
            .ToList();
    }

    public async Task MarkMovieWatchedAsync(
        Guid userId,
        Guid movieId)
    {
        await EnsureMovieExistsAsync(movieId);

        var alreadyExists =
            await watchHistoryRepository.AnyAsync(
                x =>
                    x.UserId == userId &&
                    x.MovieId == movieId);

        if (alreadyExists)
            throw new AlreadyExistsException(
                "This movie is already marked as watched.");

        var item =
            new WatchHistoryItem
            {
                UserId = userId,
                MovieId = movieId,
                TVShowId = null
            };

        await watchHistoryRepository.AddAsync(item);
        await watchHistoryRepository.SaveChangesAsync();
    }

    public async Task MarkMovieUnwatchedAsync(
        Guid userId,
        Guid movieId)
    {
        var item =
            await watchHistoryRepository.FirstOrDefaultAsync(
                x =>
                    x.UserId == userId &&
                    x.MovieId == movieId,
                true);

        if (item is null)
            throw new NotFoundException(
                "This movie is not marked as watched.");

        watchHistoryRepository.Delete(item);
        await watchHistoryRepository.SaveChangesAsync();
    }

    public async Task MarkTVShowWatchedAsync(
        Guid userId,
        Guid tvShowId)
    {
        await EnsureTVShowExistsAsync(tvShowId);

        var alreadyExists =
            await watchHistoryRepository.AnyAsync(
                x =>
                    x.UserId == userId &&
                    x.TVShowId == tvShowId);

        if (alreadyExists)
            throw new AlreadyExistsException(
                "This TV show is already marked as watched.");

        var item =
            new WatchHistoryItem
            {
                UserId = userId,
                MovieId = null,
                TVShowId = tvShowId
            };

        await watchHistoryRepository.AddAsync(item);
        await watchHistoryRepository.SaveChangesAsync();
    }

    public async Task MarkTVShowUnwatchedAsync(
        Guid userId,
        Guid tvShowId)
    {
        var item =
            await watchHistoryRepository.FirstOrDefaultAsync(
                x =>
                    x.UserId == userId &&
                    x.TVShowId == tvShowId,
                true);

        if (item is null)
            throw new NotFoundException(
                "This TV show is not marked as watched.");

        watchHistoryRepository.Delete(item);
        await watchHistoryRepository.SaveChangesAsync();
    }

    private LibraryItemReturnDto MapItem(
        WatchHistoryItem item)
    {
        if (item.Movie is not null)
        {
            return new LibraryItemReturnDto
            {
                Id = item.Id,
                ContentType = "Movie",
                ContentId = item.Movie.Id,
                Title = item.Movie.Title,

                PosterUrl =
                    mediaUrlBuilder.BuildImageUrl(
                        item.Movie.PosterUrl,
                        "movies"),

                ReleaseDate =
                    item.Movie.ReleaseDate,

                AverageRating =
                    item.Movie.Reviews.Count != 0
                        ? item.Movie.Reviews
                            .Average(x => x.Rating)
                        : null,

                ActivityAt =
                    item.WatchedAt
            };
        }

        if (item.TVShow is not null)
        {
            return new LibraryItemReturnDto
            {
                Id = item.Id,
                ContentType = "TVShow",
                ContentId = item.TVShow.Id,
                Title = item.TVShow.Title,

                PosterUrl =
                    mediaUrlBuilder.BuildImageUrl(
                        item.TVShow.PosterUrl,
                        "tvshows"),

                ReleaseDate =
                    item.TVShow.ReleaseDate,

                AverageRating =
                    item.TVShow.Reviews.Count != 0
                        ? item.TVShow.Reviews
                            .Average(x => x.Rating)
                        : null,

                ActivityAt =
                    item.WatchedAt
            };
        }

        throw new InvalidOperationException(
            "Watch history item does not reference a movie or TV show.");
    }

    private async Task EnsureMovieExistsAsync(
        Guid movieId)
    {
        var exists =
            await movieRepository.AnyAsync(
                x => x.Id == movieId);

        if (!exists)
            throw new NotFoundException(
                "Movie was not found.");
    }

    private async Task EnsureTVShowExistsAsync(
        Guid tvShowId)
    {
        var exists =
            await tvShowRepository.AnyAsync(
                x => x.Id == tvShowId);

        if (!exists)
            throw new NotFoundException(
                "TV show was not found.");
    }
}