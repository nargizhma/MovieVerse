using MovieVerse.Abstractions.Identity;
using MovieVerse.Dtos.Admin;
using MovieVerse.Exceptions;
using MovieVerse.Models;
using MovieVerse.Repositories.Interfaces;
using MovieVerse.Services.Interfaces;

namespace MovieVerse.Services;

public class AdminUserService(
    IIdentityService identityService,
    IGenericRepository<MovieReview>
        movieReviewRepository,
    IGenericRepository<TVShowReview>
        tvShowReviewRepository,
    IGenericRepository<EpisodeReview>
        episodeReviewRepository,
    IGenericRepository<WatchlistItem>
        watchlistRepository,
    IGenericRepository<WatchHistoryItem>
        watchHistoryRepository)
    : IAdminUserService
{
    public async Task<List<AdminUserReturnDto>>
        GetAllAsync()
    {
        var users =
            await identityService
                .GetAllUsersAsync();

        var result =
            new List<AdminUserReturnDto>();

        foreach (var user in users)
        {
            var movieReviews =
                await movieReviewRepository
                    .CountAsync(
                        x =>
                            x.UserId ==
                            user.Id);

            var tvReviews =
                await tvShowReviewRepository
                    .CountAsync(
                        x =>
                            x.UserId ==
                            user.Id);

            var episodeReviews =
                await episodeReviewRepository
                    .CountAsync(
                        x =>
                            x.UserId ==
                            user.Id);

            var watchlist =
                await watchlistRepository
                    .CountAsync(
                        x =>
                            x.UserId ==
                            user.Id);

            var history =
                await watchHistoryRepository
                    .CountAsync(
                        x =>
                            x.UserId ==
                            user.Id);

            result.Add(
                new AdminUserReturnDto
                {
                    Id = user.Id,

                    UserName =
                        user.UserName,

                    Email =
                        user.Email,

                    DisplayName =
                        user.DisplayName,

                    Roles =
                        user.Roles,

                    ReviewCount =
                        movieReviews +
                        tvReviews +
                        episodeReviews,

                    WatchlistCount =
                        watchlist,

                    WatchHistoryCount =
                        history
                });
        }

        return result;
    }

    public async Task SetRoleAsync(
        Guid actingUserId,
        Guid targetUserId,
        string role)
    {
        if (actingUserId ==
            targetUserId)
        {
            throw new BadRequestException(
                "You cannot change your own role.");
        }

        var normalizedRole =
            NormalizeRole(role);

        var user =
            await identityService
                .FindByIdAsync(
                    targetUserId);

        if (user is null)
            throw new NotFoundException(
                "User was not found.");

        var currentRoles =
            await identityService
                .GetRolesAsync(
                    targetUserId);

        if (currentRoles.Count == 1 &&
            string.Equals(
                currentRoles[0],
                normalizedRole,
                StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        if (currentRoles.Any(x =>
                string.Equals(
                    x,
                    "SuperAdmin",
                    StringComparison.OrdinalIgnoreCase))
            &&
            !string.Equals(
                normalizedRole,
                "SuperAdmin",
                StringComparison.OrdinalIgnoreCase))
        {
            var superAdminCount =
                await identityService
                    .CountUsersInRoleAsync(
                        "SuperAdmin");

            if (superAdminCount <= 1)
            {
                throw new BadRequestException(
                    "The last SuperAdmin cannot be demoted.");
            }
        }

        if (currentRoles.Count != 0)
        {
            var removeResult =
                await identityService
                    .RemoveFromRolesAsync(
                        targetUserId,
                        currentRoles);

            if (!removeResult.Succeeded)
            {
                throw new BadRequestException(
                    string.Join(
                        " ",
                        removeResult.Errors));
            }
        }

        var addResult =
            await identityService
                .AddToRoleAsync(
                    targetUserId,
                    normalizedRole);

        if (!addResult.Succeeded)
        {
            throw new BadRequestException(
                string.Join(
                    " ",
                    addResult.Errors));
        }
    }

    private static string NormalizeRole(
        string role)
    {
        return role.Trim()
            .ToLowerInvariant()
            switch
        {
            "user" =>
                "User",

            "admin" =>
                "Admin",

            "superadmin" =>
                "SuperAdmin",

            _ =>
                throw new BadRequestException(
                    "Role must be User, Admin, or SuperAdmin.")
        };
    }
}