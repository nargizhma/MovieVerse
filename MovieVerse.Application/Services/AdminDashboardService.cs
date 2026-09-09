using MovieVerse.Abstractions.Identity;
using MovieVerse.Dtos.Admin;
using MovieVerse.Models;
using MovieVerse.Repositories.Interfaces;
using MovieVerse.Services.Interfaces;

namespace MovieVerse.Services;

public class AdminDashboardService(
    IIdentityService identityService,
    IGenericRepository<Movie> movieRepository,
    IGenericRepository<TVShow> tvShowRepository,
    IGenericRepository<Season> seasonRepository,
    IGenericRepository<Episode> episodeRepository,
    IGenericRepository<Actor> actorRepository,
    IGenericRepository<Director> directorRepository,
    IGenericRepository<Writer> writerRepository,
    IGenericRepository<Genre> genreRepository,
    IGenericRepository<MovieReview> movieReviewRepository,
    IGenericRepository<TVShowReview> tvShowReviewRepository,
    IGenericRepository<EpisodeReview> episodeReviewRepository,
    IGenericRepository<WatchlistItem> watchlistRepository,
    IGenericRepository<WatchHistoryItem> watchHistoryRepository)
    : IAdminDashboardService
{
    public async Task<AdminDashboardStatsDto>
        GetStatsAsync()
    {
        return new AdminDashboardStatsDto
        {
            UserCount =
                await identityService
                    .CountUsersAsync(),

            AdminCount =
                await identityService
                    .CountUsersInRoleAsync(
                        "Admin"),

            SuperAdminCount =
                await identityService
                    .CountUsersInRoleAsync(
                        "SuperAdmin"),

            MovieCount =
                await movieRepository
                    .CountAsync(),

            TVShowCount =
                await tvShowRepository
                    .CountAsync(),

            SeasonCount =
                await seasonRepository
                    .CountAsync(),

            EpisodeCount =
                await episodeRepository
                    .CountAsync(),

            ActorCount =
                await actorRepository
                    .CountAsync(),

            DirectorCount =
                await directorRepository
                    .CountAsync(),

            WriterCount =
                await writerRepository
                    .CountAsync(),

            GenreCount =
                await genreRepository
                    .CountAsync(),

            MovieReviewCount =
                await movieReviewRepository
                    .CountAsync(),

            TVShowReviewCount =
                await tvShowReviewRepository
                    .CountAsync(),

            EpisodeReviewCount =
                await episodeReviewRepository
                    .CountAsync(),

            WatchlistItemCount =
                await watchlistRepository
                    .CountAsync(),

            WatchHistoryItemCount =
                await watchHistoryRepository
                    .CountAsync()
        };
    }
}