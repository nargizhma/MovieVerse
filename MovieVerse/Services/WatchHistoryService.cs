using Microsoft.EntityFrameworkCore;
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
    IHttpContextAccessor httpContextAccessor)
    : IWatchHistoryService
{
    public async Task<List<LibraryItemReturnDto>> GetMineAsync(
        Guid userId)
    {
        var items =
            await watchHistoryRepository.Query()
                .Where(x => x.UserId == userId)
                .Include(x => x.Movie)
                    .ThenInclude(x => x!.Reviews)
                .Include(x => x.TVShow)
                    .ThenInclude(x => x!.Reviews)
                .AsNoTracking()
                .ToListAsync();

        return items
            .Select(MapItem)
            .ToList();
    }

    public async Task MarkMovieWatchedAsync(
        Guid userId,
        Guid movieId)
    {
        await EnsureMovieExistsAsync(movieId);

        var alreadyExists =
            await watchHistoryRepository.Query()
                .AnyAsync(x =>
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
            await watchHistoryRepository.Query()
                .FirstOrDefaultAsync(x =>
                    x.UserId == userId &&
                    x.MovieId == movieId);

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
            await watchHistoryRepository.Query()
                .AnyAsync(x =>
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
            await watchHistoryRepository.Query()
                .FirstOrDefaultAsync(x =>
                    x.UserId == userId &&
                    x.TVShowId == tvShowId);

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
                PosterUrl = BuildPosterUrl(
                    item.Movie.PosterUrl,
                    "movies"),
                ReleaseDate = item.Movie.ReleaseDate,
                AverageRating =
                    item.Movie.Reviews.Count != 0
                        ? item.Movie.Reviews
                            .Average(x => x.Rating)
                        : null
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
                PosterUrl = BuildPosterUrl(
                    item.TVShow.PosterUrl,
                    "tvshows"),
                ReleaseDate = item.TVShow.ReleaseDate,
                AverageRating =
                    item.TVShow.Reviews.Count != 0
                        ? item.TVShow.Reviews
                            .Average(x => x.Rating)
                        : null
            };
        }

        throw new InvalidOperationException(
            "Watch history item does not reference a movie or TV show.");
    }

    private string? BuildPosterUrl(
        string? fileName,
        string folder)
    {
        if (string.IsNullOrWhiteSpace(fileName))
            return null;

        var relativeUrl =
            $"/images/{folder}/{fileName}";

        var request =
            httpContextAccessor
                .HttpContext?
                .Request;

        if (request is null)
            return relativeUrl;

        return
            $"{request.Scheme}://{request.Host}{relativeUrl}";
    }

    private async Task EnsureMovieExistsAsync(
        Guid movieId)
    {
        var exists =
            await movieRepository.Query()
                .AnyAsync(x => x.Id == movieId);

        if (!exists)
            throw new NotFoundException(
                "Movie was not found.");
    }

    private async Task EnsureTVShowExistsAsync(
        Guid tvShowId)
    {
        var exists =
            await tvShowRepository.Query()
                .AnyAsync(x => x.Id == tvShowId);

        if (!exists)
            throw new NotFoundException(
                "TV show was not found.");
    }
}
