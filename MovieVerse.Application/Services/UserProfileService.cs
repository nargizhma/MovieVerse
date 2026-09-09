using MovieVerse.Abstractions.Identity;
using MovieVerse.Abstractions.Media;
using MovieVerse.Abstractions.Persistence;
using MovieVerse.Dtos.Profiles;
using MovieVerse.Exceptions;
using MovieVerse.Models;
using MovieVerse.Repositories.Interfaces;
using MovieVerse.Requests.Profiles;
using MovieVerse.Services.Interfaces;

namespace MovieVerse.Services;

public class UserProfileService(
    IIdentityService identityService,
    IUserProfileRepository profileRepository,
    IGenericRepository<MovieReview>
        movieReviewRepository,
    IGenericRepository<TVShowReview>
        tvShowReviewRepository,
    IGenericRepository<EpisodeReview>
        episodeReviewRepository,
    IGenericRepository<WatchlistItem>
        watchlistRepository,
    IGenericRepository<WatchHistoryItem>
        watchHistoryRepository,
    IFileStorage fileStorage,
    IMediaUrlBuilder mediaUrlBuilder,
    IUnitOfWork unitOfWork)
    : IUserProfileService
{
    public async Task<MyProfileReturnDto>
        GetMineAsync(Guid userId)
    {
        var user =
            await identityService
                .FindByIdAsync(userId);

        if (user is null)
            throw new NotFoundException(
                "User was not found.");

        var counts =
            await GetCountsAsync(user.Id);

        return new MyProfileReturnDto
        {
            UserId = user.Id,
            UserName = user.UserName,
            Email = user.Email,
            DisplayName =
                user.DisplayName,
            Bio = user.Bio,

            ProfileImageUrl =
                mediaUrlBuilder
                    .BuildImageUrl(
                        user.ProfileImageUrl,
                        "profiles"),

            MovieReviewCount =
                counts.MovieReviewCount,

            TVShowReviewCount =
                counts.TVShowReviewCount,

            EpisodeReviewCount =
                counts.EpisodeReviewCount,

            WatchlistCount =
                counts.WatchlistCount,

            WatchHistoryCount =
                counts.WatchHistoryCount
        };
    }

    public async Task<UserProfileReturnDto>
        GetByUserNameAsync(
            string userName)
    {
        var user =
            await identityService
                .FindByUserNameAsync(
                    userName.Trim());

        if (user is null)
            throw new NotFoundException(
                "User was not found.");

        var counts =
            await GetCountsAsync(user.Id);

        return new UserProfileReturnDto
        {
            UserId = user.Id,
            UserName = user.UserName,

            DisplayName =
                user.DisplayName,

            Bio =
                user.Bio,

            ProfileImageUrl =
                mediaUrlBuilder
                    .BuildImageUrl(
                        user.ProfileImageUrl,
                        "profiles"),

            MovieReviewCount =
                counts.MovieReviewCount,

            TVShowReviewCount =
                counts.TVShowReviewCount,

            EpisodeReviewCount =
                counts.EpisodeReviewCount,

            WatchlistCount =
                counts.WatchlistCount,

            WatchHistoryCount =
                counts.WatchHistoryCount
        };
    }

    public async Task UpdateAsync(
        Guid userId,
        ProfileUpdateRequest request)
    {
        var user =
            await identityService
                .FindByIdAsync(userId);

        if (user is null)
            throw new NotFoundException(
                "User was not found.");

        var profile =
            await profileRepository
                .GetByUserIdAsync(
                    userId,
                    true);

        if (profile is null)
        {
            profile =
                new UserProfile
                {
                    AppUserId = userId
                };

            await profileRepository
                .AddAsync(profile);
        }

        var oldImage =
            profile.ProfileImageUrl;

        profile.DisplayName =
            NormalizeOptionalText(
                request.DisplayName);

        profile.Bio =
            NormalizeOptionalText(
                request.Bio);

        string? newImage = null;

        if (request.ProfileImage is not null)
        {
            newImage =
                await fileStorage.SaveAsync(
                    request.ProfileImage,
                    "profiles");

            profile.ProfileImageUrl =
                newImage;
        }

        try
        {
            await unitOfWork
                .SaveChangesAsync();
        }
        catch
        {
            fileStorage.Delete(
                newImage,
                "profiles");

            throw;
        }

        if (newImage is not null)
        {
            fileStorage.Delete(
                oldImage,
                "profiles");
        }
    }

    public async Task
        DeleteProfileImageAsync(
            Guid userId)
    {
        var profile =
            await profileRepository
                .GetByUserIdAsync(
                    userId,
                    true);

        if (profile is null)
            throw new NotFoundException(
                "User profile was not found.");

        if (string.IsNullOrWhiteSpace(
                profile.ProfileImageUrl))
        {
            return;
        }

        var oldImage =
            profile.ProfileImageUrl;

        profile.ProfileImageUrl =
            null;

        await unitOfWork
            .SaveChangesAsync();

        fileStorage.Delete(
            oldImage,
            "profiles");
    }

    public async Task<List<ProfileActivityItemDto>>
        GetMyActivityAsync(
            Guid userId)
    {
        var movieReviews =
            await movieReviewRepository
                .FindAllAsync(
                    x =>
                        x.UserId ==
                        userId,
                    false,
                    "Movie");

        var tvShowReviews =
            await tvShowReviewRepository
                .FindAllAsync(
                    x =>
                        x.UserId ==
                        userId,
                    false,
                    "TVShow");

        var episodeReviews =
            await episodeReviewRepository
                .FindAllAsync(
                    x =>
                        x.UserId ==
                        userId,
                    false,
                    "Episode.Season.TVShow");

        var result =
            new List<ProfileActivityItemDto>();

        result.AddRange(
            movieReviews.Select(x =>
                new ProfileActivityItemDto
                {
                    ReviewId = x.Id,
                    ContentType = "Movie",
                    ContentId = x.MovieId,
                    Title = x.Movie.Title,

                    ImageUrl =
                        mediaUrlBuilder
                            .BuildImageUrl(
                                x.Movie.PosterUrl,
                                "movies"),

                    Rating = x.Rating,
                    Content = x.Content,

                    ActivityAt =
                        x.UpdatedAt
                        ?? x.CreatedAt
                }));

        result.AddRange(
            tvShowReviews.Select(x =>
                new ProfileActivityItemDto
                {
                    ReviewId = x.Id,
                    ContentType = "TVShow",
                    ContentId = x.TVShowId,
                    Title = x.TVShow.Title,

                    ImageUrl =
                        mediaUrlBuilder
                            .BuildImageUrl(
                                x.TVShow.PosterUrl,
                                "tvshows"),

                    Rating = x.Rating,
                    Content = x.Content,

                    ActivityAt =
                        x.UpdatedAt
                        ?? x.CreatedAt
                }));

        result.AddRange(
            episodeReviews.Select(x =>
                new ProfileActivityItemDto
                {
                    ReviewId = x.Id,
                    ContentType = "Episode",
                    ContentId = x.EpisodeId,

                    TVShowId =
                        x.Episode
                            .Season
                            .TVShowId,

                    Title =
                        x.Episode.Title,

                    ParentTitle =
                        x.Episode
                            .Season
                            .TVShow
                            .Title,

                    SeasonNumber =
                        x.Episode
                            .Season
                            .SeasonNumber,

                    EpisodeNumber =
                        x.Episode
                            .EpisodeNumber,

                    ImageUrl =
                        mediaUrlBuilder
                            .BuildImageUrl(
                                x.Episode.ImageUrl,
                                "episodes"),

                    Rating = x.Rating,
                    Content = x.Content,

                    ActivityAt =
                        x.UpdatedAt
                        ?? x.CreatedAt
                }));

        return result
            .OrderByDescending(x =>
                x.ActivityAt)
            .ToList();
    }

    private async Task<ProfileCounts>
        GetCountsAsync(Guid userId)
    {
        var movieReviewCount =
            await movieReviewRepository
                .CountAsync(
                    x =>
                        x.UserId ==
                        userId);

        var tvShowReviewCount =
            await tvShowReviewRepository
                .CountAsync(
                    x =>
                        x.UserId ==
                        userId);

        var episodeReviewCount =
            await episodeReviewRepository
                .CountAsync(
                    x =>
                        x.UserId ==
                        userId);

        var watchlistCount =
            await watchlistRepository
                .CountAsync(
                    x =>
                        x.UserId ==
                        userId);

        var watchHistoryCount =
            await watchHistoryRepository
                .CountAsync(
                    x =>
                        x.UserId ==
                        userId);

        return new ProfileCounts(
            movieReviewCount,
            tvShowReviewCount,
            episodeReviewCount,
            watchlistCount,
            watchHistoryCount);
    }

    private static string?
        NormalizeOptionalText(
            string? value)
    {
        return string.IsNullOrWhiteSpace(
                value)
            ? null
            : value.Trim();
    }

    private sealed record ProfileCounts(
        int MovieReviewCount,
        int TVShowReviewCount,
        int EpisodeReviewCount,
        int WatchlistCount,
        int WatchHistoryCount);
}