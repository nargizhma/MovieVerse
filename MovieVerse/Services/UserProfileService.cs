using Microsoft.EntityFrameworkCore;
using MovieVerse.Data;
using MovieVerse.Dtos.Profiles;
using MovieVerse.Exceptions;
using MovieVerse.Extensions;
using MovieVerse.Models;
using MovieVerse.Services.Interfaces;

namespace MovieVerse.Services;

public class UserProfileService(
    AppDbContext dbContext,
    IWebHostEnvironment environment,
    IHttpContextAccessor httpContextAccessor)
    : IUserProfileService
{
    public async Task<MyProfileReturnDto> GetMineAsync(
        Guid userId)
    {
        var user =
            await dbContext.Users
                .Include(x => x.Profile)
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    x => x.Id == userId);

        if (user is null)
            throw new NotFoundException(
                "User was not found.");

        var counts =
            await GetCountsAsync(user.Id);

        return new MyProfileReturnDto
        {
            UserId = user.Id,
            UserName = user.UserName ?? string.Empty,
            Email = user.Email ?? string.Empty,
            DisplayName = user.Profile?.DisplayName,
            Bio = user.Profile?.Bio,
            ProfileImageUrl =
                httpContextAccessor.BuildImageUrl(
                    user.Profile?.ProfileImageUrl,
                    "profiles"),
            MovieReviewCount = counts.MovieReviewCount,
            TVShowReviewCount = counts.TVShowReviewCount,
            EpisodeReviewCount = counts.EpisodeReviewCount,
            WatchlistCount = counts.WatchlistCount,
            WatchHistoryCount = counts.WatchHistoryCount
        };
    }

    public async Task<UserProfileReturnDto> GetByUserNameAsync(
        string userName)
    {
        var normalizedUserName =
            userName.Trim();

        var user =
            await dbContext.Users
                .Include(x => x.Profile)
                .AsNoTracking()
                .FirstOrDefaultAsync(x =>
                    x.UserName == normalizedUserName);

        if (user is null)
            throw new NotFoundException(
                "User was not found.");

        var counts =
            await GetCountsAsync(user.Id);

        return new UserProfileReturnDto
        {
            UserId = user.Id,
            UserName = user.UserName ?? string.Empty,
            DisplayName = user.Profile?.DisplayName,
            Bio = user.Profile?.Bio,
            ProfileImageUrl =
                httpContextAccessor.BuildImageUrl(
                    user.Profile?.ProfileImageUrl,
                    "profiles"),
            MovieReviewCount = counts.MovieReviewCount,
            TVShowReviewCount = counts.TVShowReviewCount,
            EpisodeReviewCount = counts.EpisodeReviewCount,
            WatchlistCount = counts.WatchlistCount,
            WatchHistoryCount = counts.WatchHistoryCount
        };
    }

    public async Task UpdateAsync(
        Guid userId,
        ProfileUpdateDto dto)
    {
        var user =
            await dbContext.Users
                .Include(x => x.Profile)
                .FirstOrDefaultAsync(
                    x => x.Id == userId);

        if (user is null)
            throw new NotFoundException(
                "User was not found.");

        if (user.Profile is null)
        {
            user.Profile =
                new UserProfile
                {
                    AppUserId = user.Id
                };
        }

        var oldImage =
            user.Profile.ProfileImageUrl;

        user.Profile.DisplayName =
            NormalizeOptionalText(
                dto.DisplayName);

        user.Profile.Bio =
            NormalizeOptionalText(
                dto.Bio);

        var folderPath =
            environment.GetImageFolderPath(
                "profiles");

        string? newImage = null;

        if (dto.ProfileImage is not null)
        {
            newImage =
                await dto.ProfileImage
                    .SaveFileAsync(
                        folderPath);

            user.Profile.ProfileImageUrl =
                newImage;
        }

        try
        {
            await dbContext.SaveChangesAsync();
        }
        catch
        {
            FileManager.DeleteFile(
                newImage,
                folderPath);

            throw;
        }

        if (newImage is not null)
        {
            FileManager.DeleteFile(
                oldImage,
                folderPath);
        }
    }

    public async Task DeleteProfileImageAsync(
        Guid userId)
    {
        var profile =
            await dbContext.UserProfiles
                .FirstOrDefaultAsync(x =>
                    x.AppUserId == userId);

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

        await dbContext.SaveChangesAsync();

        FileManager.DeleteFile(
            oldImage,
            environment.GetImageFolderPath(
                "profiles"));
    }

    public async Task<List<ProfileActivityItemDto>> GetMyActivityAsync(
        Guid userId)
    {
        var movieReviews =
            await dbContext.MovieReviews
                .Where(x =>
                    x.UserId == userId)
                .Include(x => x.Movie)
                .AsNoTracking()
                .ToListAsync();

        var tvShowReviews =
            await dbContext.TVShowReviews
                .Where(x =>
                    x.UserId == userId)
                .Include(x => x.TVShow)
                .AsNoTracking()
                .ToListAsync();

        var episodeReviews =
            await dbContext.EpisodeReviews
                .Where(x =>
                    x.UserId == userId)
                .Include(x => x.Episode)
                    .ThenInclude(x => x.Season)
                    .ThenInclude(x => x.TVShow)
                .AsNoTracking()
                .ToListAsync();

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
                        httpContextAccessor.BuildImageUrl(
                            x.Movie.PosterUrl,
                            "movies"),
                    Rating = x.Rating,
                    Content = x.Content,
                    ActivityAt =
                        x.UpdatedAt ?? x.CreatedAt
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
                        httpContextAccessor.BuildImageUrl(
                            x.TVShow.PosterUrl,
                            "tvshows"),
                    Rating = x.Rating,
                    Content = x.Content,
                    ActivityAt =
                        x.UpdatedAt ?? x.CreatedAt
                }));

        result.AddRange(
            episodeReviews.Select(x =>
                new ProfileActivityItemDto
                {
                    ReviewId = x.Id,
                    ContentType = "Episode",
                    ContentId = x.EpisodeId,
                    TVShowId =
                        x.Episode.Season.TVShowId,
                    Title = x.Episode.Title,
                    ParentTitle =
                        x.Episode.Season.TVShow.Title,
                    SeasonNumber =
                        x.Episode.Season.SeasonNumber,
                    EpisodeNumber =
                        x.Episode.EpisodeNumber,
                    ImageUrl =
                        httpContextAccessor.BuildImageUrl(
                            x.Episode.ImageUrl,
                            "episodes"),
                    Rating = x.Rating,
                    Content = x.Content,
                    ActivityAt =
                        x.UpdatedAt ?? x.CreatedAt
                }));

        return result
            .OrderByDescending(x =>
                x.ActivityAt)
            .ToList();
    }

    private async Task<ProfileCounts> GetCountsAsync(
        Guid userId)
    {
        var movieReviewCount =
            await dbContext.MovieReviews
                .CountAsync(x =>
                    x.UserId == userId);

        var tvShowReviewCount =
            await dbContext.TVShowReviews
                .CountAsync(x =>
                    x.UserId == userId);

        var episodeReviewCount =
            await dbContext.EpisodeReviews
                .CountAsync(x =>
                    x.UserId == userId);

        var watchlistCount =
            await dbContext.WatchlistItems
                .CountAsync(x =>
                    x.UserId == userId);

        var watchHistoryCount =
            await dbContext.WatchHistoryItems
                .CountAsync(x =>
                    x.UserId == userId);

        return new ProfileCounts(
            movieReviewCount,
            tvShowReviewCount,
            episodeReviewCount,
            watchlistCount,
            watchHistoryCount);
    }

    private static string? NormalizeOptionalText(
        string? value)
    {
        return string.IsNullOrWhiteSpace(value)
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
