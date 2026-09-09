using MovieVerse.Abstractions.Identity;
using MovieVerse.Dtos.Admin;
using MovieVerse.Exceptions;
using MovieVerse.Models;
using MovieVerse.Repositories.Interfaces;
using MovieVerse.Services.Interfaces;

namespace MovieVerse.Services;

public class AdminReviewService(
    IGenericRepository<MovieReview> movieReviewRepository,
    IGenericRepository<TVShowReview> tvShowReviewRepository,
    IGenericRepository<EpisodeReview> episodeReviewRepository,
    IIdentityService identityService)
    : IAdminReviewService
{
    public async Task<List<AdminReviewReturnDto>>
        GetAllAsync(string? type)
    {
        var normalizedType =
            NormalizeOptionalType(type);

        var users =
            (await identityService.GetAllUsersAsync())
            .ToDictionary(x => x.Id);

        var result =
            new List<AdminReviewReturnDto>();

        if (normalizedType is null ||
            normalizedType == "movie")
        {
            var reviews =
                await movieReviewRepository.FindAllAsync(
                    x => true,
                    false,
                    "Movie");

            result.AddRange(
                reviews.Select(x =>
                {
                    users.TryGetValue(
                        x.UserId,
                        out var user);

                    return new AdminReviewReturnDto
                    {
                        Id = x.Id,
                        ContentType = "Movie",
                        ContentId = x.MovieId,
                        Title = x.Movie.Title,
                        UserId = x.UserId,
                        UserName =
                            user?.UserName
                            ?? string.Empty,
                        DisplayName =
                            user?.DisplayName,
                        Rating = x.Rating,
                        Content = x.Content
                    };
                }));
        }

        if (normalizedType is null ||
            normalizedType == "tvshow")
        {
            var reviews =
                await tvShowReviewRepository.FindAllAsync(
                    x => true,
                    false,
                    "TVShow");

            result.AddRange(
                reviews.Select(x =>
                {
                    users.TryGetValue(
                        x.UserId,
                        out var user);

                    return new AdminReviewReturnDto
                    {
                        Id = x.Id,
                        ContentType = "TVShow",
                        ContentId = x.TVShowId,
                        Title = x.TVShow.Title,
                        UserId = x.UserId,
                        UserName =
                            user?.UserName
                            ?? string.Empty,
                        DisplayName =
                            user?.DisplayName,
                        Rating = x.Rating,
                        Content = x.Content
                    };
                }));
        }

        if (normalizedType is null ||
            normalizedType == "episode")
        {
            var reviews =
                await episodeReviewRepository.FindAllAsync(
                    x => true,
                    false,
                    "Episode.Season.TVShow");

            result.AddRange(
                reviews.Select(x =>
                {
                    users.TryGetValue(
                        x.UserId,
                        out var user);

                    return new AdminReviewReturnDto
                    {
                        Id = x.Id,
                        ContentType = "Episode",
                        ContentId = x.EpisodeId,

                        Title =
                            $"{x.Episode.Season.TVShow.Title} - " +
                            $"S{x.Episode.Season.SeasonNumber}" +
                            $"E{x.Episode.EpisodeNumber}: " +
                            x.Episode.Title,

                        UserId = x.UserId,

                        UserName =
                            user?.UserName
                            ?? string.Empty,

                        DisplayName =
                            user?.DisplayName,

                        Rating = x.Rating,
                        Content = x.Content
                    };
                }));
        }

        return result
            .OrderBy(x => x.ContentType)
            .ThenBy(x => x.Title)
            .ThenBy(x => x.UserName)
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
                        await movieReviewRepository
                            .FirstOrDefaultAsync(
                                x => x.Id == reviewId,
                                true);

                    if (review is null)
                        throw new NotFoundException(
                            "Movie review was not found.");

                    movieReviewRepository.Delete(review);

                    await movieReviewRepository
                        .SaveChangesAsync();

                    break;
                }

            case "tvshow":
                {
                    var review =
                        await tvShowReviewRepository
                            .FirstOrDefaultAsync(
                                x => x.Id == reviewId,
                                true);

                    if (review is null)
                        throw new NotFoundException(
                            "TV show review was not found.");

                    tvShowReviewRepository.Delete(review);

                    await tvShowReviewRepository
                        .SaveChangesAsync();

                    break;
                }

            case "episode":
                {
                    var review =
                        await episodeReviewRepository
                            .FirstOrDefaultAsync(
                                x => x.Id == reviewId,
                                true);

                    if (review is null)
                        throw new NotFoundException(
                            "Episode review was not found.");

                    episodeReviewRepository.Delete(review);

                    await episodeReviewRepository
                        .SaveChangesAsync();

                    break;
                }
        }
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
        return type.Trim()
            .ToLowerInvariant()
            switch
        {
            "movie" => "movie",
            "tvshow" => "tvshow",
            "tv-show" => "tvshow",
            "tv_show" => "tvshow",
            "episode" => "episode",

            _ =>
                throw new BadRequestException(
                    "Review type must be movie, tvshow, or episode.")
        };
    }
}