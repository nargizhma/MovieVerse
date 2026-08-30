using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MovieVerse.Data;
using MovieVerse.Dtos.Admin;
using MovieVerse.Models;
using MovieVerse.Services.Interfaces;

namespace MovieVerse.Services;

public class AdminDashboardService(
    AppDbContext dbContext,
    UserManager<AppUser> userManager)
    : IAdminDashboardService
{
    public async Task<AdminDashboardStatsDto>
        GetStatsAsync()
    {
        var adminUsers =
            await userManager
                .GetUsersInRoleAsync("Admin");

        var superAdminUsers =
            await userManager
                .GetUsersInRoleAsync("SuperAdmin");

        return new AdminDashboardStatsDto
        {
            UserCount =
                await dbContext.Users.CountAsync(),

            AdminCount =
                adminUsers.Count,

            SuperAdminCount =
                superAdminUsers.Count,

            MovieCount =
                await dbContext.Movies.CountAsync(),

            TVShowCount =
                await dbContext.TVShows.CountAsync(),

            SeasonCount =
                await dbContext.Seasons.CountAsync(),

            EpisodeCount =
                await dbContext.Episodes.CountAsync(),

            ActorCount =
                await dbContext.Actors.CountAsync(),

            DirectorCount =
                await dbContext.Directors.CountAsync(),

            WriterCount =
                await dbContext.Writers.CountAsync(),

            GenreCount =
                await dbContext.Genres.CountAsync(),

            MovieReviewCount =
                await dbContext.MovieReviews.CountAsync(),

            TVShowReviewCount =
                await dbContext.TVShowReviews.CountAsync(),

            EpisodeReviewCount =
                await dbContext.EpisodeReviews.CountAsync(),

            WatchlistItemCount =
                await dbContext.WatchlistItems.CountAsync(),

            WatchHistoryItemCount =
                await dbContext.WatchHistoryItems.CountAsync()
        };
    }
}
