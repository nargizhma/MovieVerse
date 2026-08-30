using Microsoft.EntityFrameworkCore;
using MovieVerse.Data;
using MovieVerse.Dtos.Admin;
using MovieVerse.Exceptions;
using MovieVerse.Services.Interfaces;

namespace MovieVerse.Services;

public class AdminReviewService(
    AppDbContext dbContext)
    : IAdminReviewService
{
    public async Task<List<AdminReviewReturnDto>>
        GetAllAsync(
            string? type)
    {
        var normalizedType =
            NormalizeOptionalType(type);

        var result =
            new List<AdminReviewReturnDto>();

        if (normalizedType is null ||
            normalizedType == "movie")
        {
            var movieReviews =
                await dbContext.MovieReviews
                    .Include(x => x.Movie)
                    .Include(x => x.User)
                        .ThenInclude(x => x.Profile)
                    .AsNoTracking()
                    .ToListAsync();

            result.AddRange(
                movieReviews.Select(x =>
                    new AdminReviewReturnDto
                    {
                        Id =
                            x.Id,

                        ContentType =
                            "Movie",

                        ContentId =
                            x.MovieId,

                        Title =
                            x.Movie.Title,

                        UserId =
                            x.UserId,

                        UserName =
                            x.User.UserName
                            ?? string.Empty,

                        DisplayName =
                            x.User.Profile?.DisplayName,

                        Rating =
                            x.Rating,

                        Content =
                            x.Content
                    }));
        }

        if (normalizedType is null ||
            normalizedType == "tvshow")
        {
            var tvShowReviews =
                await dbContext.TVShowReviews
                    .Include(x => x.TVShow)
                    .Include(x => x.User)
                        .ThenInclude(x => x.Profile)
                    .AsNoTracking()
                    .ToListAsync();

            result.AddRange(
                tvShowReviews.Select(x =>
                    new AdminReviewReturnDto
                    {
                        Id =
                            x.Id,

                        ContentType =
                            "TVShow",

                        ContentId =
                            x.TVShowId,

                        Title =
                            x.TVShow.Title,

                        UserId =
                            x.UserId,

                        UserName =
                            x.User.UserName
                            ?? string.Empty,

                        DisplayName =
                            x.User.Profile?.DisplayName,

                        Rating =
                            x.Rating,

                        Content =
                            x.Content
                    }));
        }

        if (normalizedType is null ||
            normalizedType == "episode")
        {
            var episodeReviews =
                await dbContext.EpisodeReviews
                    .Include(x => x.Episode)
                        .ThenInclude(x =>
                            x.Season)
                        .ThenInclude(x =>
                            x.TVShow)
                    .Include(x => x.User)
                        .ThenInclude(x => x.Profile)
                    .AsNoTracking()
                    .ToListAsync();

            result.AddRange(
                episodeReviews.Select(x =>
                    new AdminReviewReturnDto
                    {
                        Id =
                            x.Id,

                        ContentType =
                            "Episode",

                        ContentId =
                            x.EpisodeId,

                        Title =
                            $"{x.Episode.Season.TVShow.Title} - " +
                            $"S{x.Episode.Season.SeasonNumber}E{x.Episode.EpisodeNumber}: " +
                            x.Episode.Title,

                        UserId =
                            x.UserId,

                        UserName =
                            x.User.UserName
                            ?? string.Empty,

                        DisplayName =
                            x.User.Profile?.DisplayName,

                        Rating =
                            x.Rating,

                        Content =
                            x.Content
                    }));
        }

        return result
            .OrderBy(x =>
                x.ContentType)
            .ThenBy(x =>
                x.Title)
            .ThenBy(x =>
                x.UserName)
            .ToList();
    }

    public async Task DeleteAsync(
        string type,
        Guid reviewId)
    {
        switch (NormalizeRequiredType(type))
        {
            case "movie":
            {
                var review =
                    await dbContext.MovieReviews
                        .FirstOrDefaultAsync(
                            x => x.Id == reviewId);

                if (review is null)
                    throw new NotFoundException(
                        "Movie review was not found.");

                dbContext.MovieReviews.Remove(
                    review);

                break;
            }

            case "tvshow":
            {
                var review =
                    await dbContext.TVShowReviews
                        .FirstOrDefaultAsync(
                            x => x.Id == reviewId);

                if (review is null)
                    throw new NotFoundException(
                        "TV show review was not found.");

                dbContext.TVShowReviews.Remove(
                    review);

                break;
            }

            case "episode":
            {
                var review =
                    await dbContext.EpisodeReviews
                        .FirstOrDefaultAsync(
                            x => x.Id == reviewId);

                if (review is null)
                    throw new NotFoundException(
                        "Episode review was not found.");

                dbContext.EpisodeReviews.Remove(
                    review);

                break;
            }
        }

        await dbContext.SaveChangesAsync();
    }

    private static string? NormalizeOptionalType(
        string? type)
    {
        if (string.IsNullOrWhiteSpace(type))
            return null;

        return NormalizeRequiredType(type);
    }

    private static string NormalizeRequiredType(
        string type)
    {
        return type.Trim().ToLowerInvariant() switch
        {
            "movie" =>
                "movie",

            "tvshow" =>
                "tvshow",

            "tv-show" =>
                "tvshow",

            "tv_show" =>
                "tvshow",

            "episode" =>
                "episode",

            _ =>
                throw new BadRequestException(
                    "Review type must be movie, tvshow, or episode.")
        };
    }
}
