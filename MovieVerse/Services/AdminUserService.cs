using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MovieVerse.Data;
using MovieVerse.Dtos.Admin;
using MovieVerse.Exceptions;
using MovieVerse.Models;
using MovieVerse.Services.Interfaces;

namespace MovieVerse.Services;

public class AdminUserService(
    AppDbContext dbContext,
    UserManager<AppUser> userManager)
    : IAdminUserService
{
    public async Task<List<AdminUserReturnDto>>
        GetAllAsync()
    {
        var users =
            await dbContext.Users
                .Include(x => x.Profile)
                .AsNoTracking()
                .OrderBy(x => x.UserName)
                .ToListAsync();

        var roleRows =
            await (
                from userRole in dbContext.UserRoles
                join role in dbContext.Roles
                    on userRole.RoleId equals role.Id
                select new
                {
                    userRole.UserId,
                    Role =
                        role.Name ?? string.Empty
                })
                .AsNoTracking()
                .ToListAsync();

        var rolesByUser =
            roleRows
                .GroupBy(x => x.UserId)
                .ToDictionary(
                    x => x.Key,
                    x => x
                        .Select(role =>
                            role.Role)
                        .Where(role =>
                            !string.IsNullOrWhiteSpace(
                                role))
                        .OrderBy(role =>
                            role)
                        .ToList());

        var movieReviewCounts =
            await dbContext.MovieReviews
                .GroupBy(x => x.UserId)
                .Select(x => new
                {
                    UserId = x.Key,
                    Count = x.Count()
                })
                .ToDictionaryAsync(
                    x => x.UserId,
                    x => x.Count);

        var tvShowReviewCounts =
            await dbContext.TVShowReviews
                .GroupBy(x => x.UserId)
                .Select(x => new
                {
                    UserId = x.Key,
                    Count = x.Count()
                })
                .ToDictionaryAsync(
                    x => x.UserId,
                    x => x.Count);

        var episodeReviewCounts =
            await dbContext.EpisodeReviews
                .GroupBy(x => x.UserId)
                .Select(x => new
                {
                    UserId = x.Key,
                    Count = x.Count()
                })
                .ToDictionaryAsync(
                    x => x.UserId,
                    x => x.Count);

        var watchlistCounts =
            await dbContext.WatchlistItems
                .GroupBy(x => x.UserId)
                .Select(x => new
                {
                    UserId = x.Key,
                    Count = x.Count()
                })
                .ToDictionaryAsync(
                    x => x.UserId,
                    x => x.Count);

        var watchHistoryCounts =
            await dbContext.WatchHistoryItems
                .GroupBy(x => x.UserId)
                .Select(x => new
                {
                    UserId = x.Key,
                    Count = x.Count()
                })
                .ToDictionaryAsync(
                    x => x.UserId,
                    x => x.Count);

        return users
            .Select(user =>
                new AdminUserReturnDto
                {
                    Id =
                        user.Id,

                    UserName =
                        user.UserName
                        ?? string.Empty,

                    Email =
                        user.Email
                        ?? string.Empty,

                    DisplayName =
                        user.Profile?.DisplayName,

                    Roles =
                        rolesByUser.GetValueOrDefault(
                            user.Id,
                            []),

                    ReviewCount =
                        movieReviewCounts
                            .GetValueOrDefault(
                                user.Id)
                        +
                        tvShowReviewCounts
                            .GetValueOrDefault(
                                user.Id)
                        +
                        episodeReviewCounts
                            .GetValueOrDefault(
                                user.Id),

                    WatchlistCount =
                        watchlistCounts
                            .GetValueOrDefault(
                                user.Id),

                    WatchHistoryCount =
                        watchHistoryCounts
                            .GetValueOrDefault(
                                user.Id)
                })
            .ToList();
    }

    public async Task SetRoleAsync(
        Guid actingUserId,
        Guid targetUserId,
        string role)
    {
        if (actingUserId == targetUserId)
        {
            throw new BadRequestException(
                "You cannot change your own role.");
        }

        var normalizedRole =
            NormalizeRole(role);

        var user =
            await userManager
                .FindByIdAsync(
                    targetUserId.ToString());

        if (user is null)
            throw new NotFoundException(
                "User was not found.");

        var currentRoles =
            await userManager
                .GetRolesAsync(user);

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
            var superAdmins =
                await userManager
                    .GetUsersInRoleAsync(
                        "SuperAdmin");

            if (superAdmins.Count <= 1)
            {
                throw new BadRequestException(
                    "The last SuperAdmin cannot be demoted.");
            }
        }

        if (currentRoles.Count != 0)
        {
            var removeResult =
                await userManager
                    .RemoveFromRolesAsync(
                        user,
                        currentRoles);

            if (!removeResult.Succeeded)
            {
                throw new BadRequestException(
                    JoinIdentityErrors(
                        removeResult));
            }
        }

        var addResult =
            await userManager
                .AddToRoleAsync(
                    user,
                    normalizedRole);

        if (!addResult.Succeeded)
        {
            throw new BadRequestException(
                JoinIdentityErrors(
                    addResult));
        }
    }

    private static string NormalizeRole(
        string role)
    {
        return role.Trim().ToLowerInvariant() switch
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

    private static string JoinIdentityErrors(
        IdentityResult result)
    {
        return string.Join(
            " ",
            result.Errors.Select(x =>
                x.Description));
    }
}
