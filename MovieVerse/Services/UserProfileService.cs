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
            UserId =
                user.Id,

            UserName =
                user.UserName ?? string.Empty,

            Email =
                user.Email ?? string.Empty,

            DisplayName =
                user.Profile?.DisplayName,

            Bio =
                user.Profile?.Bio,

            ProfileImageUrl =
                BuildProfileImageUrl(
                    user.Profile?.ProfileImageUrl),

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
            UserId =
                user.Id,

            UserName =
                user.UserName ?? string.Empty,

            DisplayName =
                user.Profile?.DisplayName,

            Bio =
                user.Profile?.Bio,

            ProfileImageUrl =
                BuildProfileImageUrl(
                    user.Profile?.ProfileImageUrl),

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
                    AppUserId =
                        user.Id
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

        if (dto.ProfileImage is not null)
        {
            user.Profile.ProfileImageUrl =
                await dto.ProfileImage
                    .SaveFileAsync(
                        GetImageFolderPath());
        }

        await dbContext.SaveChangesAsync();

        if (dto.ProfileImage is not null)
        {
            FileManager.DeleteFile(
                oldImage,
                GetImageFolderPath());
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
            GetImageFolderPath());
    }

    private async Task<ProfileCounts>
        GetCountsAsync(
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

    private string? BuildProfileImageUrl(
        string? fileName)
    {
        if (string.IsNullOrWhiteSpace(
                fileName))
            return null;

        var relativeUrl =
            $"/images/profiles/{fileName}";

        var request =
            httpContextAccessor
                .HttpContext?
                .Request;

        if (request is null)
            return relativeUrl;

        return
            $"{request.Scheme}://{request.Host}{relativeUrl}";
    }

    private string GetImageFolderPath()
    {
        var webRootPath =
            environment.WebRootPath
            ?? Path.Combine(
                environment.ContentRootPath,
                "wwwroot");

        return Path.Combine(
            webRootPath,
            "images",
            "profiles");
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
