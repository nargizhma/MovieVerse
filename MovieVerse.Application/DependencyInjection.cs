using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using MovieVerse.Services;
using MovieVerse.Services.Interfaces;

namespace MovieVerse.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(
        this IServiceCollection services)
    {
        services.AddScoped<
            IAuthService,
            AuthService>();

        services.AddScoped<
            IFilmChatService,
            FilmChatService>();


        services.AddScoped<
            IMovieService,
            MovieService>();

        services.AddScoped<
            ITVShowService,
            TVShowService>();


        services.AddScoped<
            IActorService,
            ActorService>();

        services.AddScoped<
            IDirectorService,
            DirectorService>();

        services.AddScoped<
            IWriterService,
            WriterService>();


        services.AddScoped<
            IGenreService,
            GenreService>();


        services.AddScoped<
            ISeasonService,
            SeasonService>();

        services.AddScoped<
            IEpisodeService,
            EpisodeService>();


        services.AddScoped<
            IMovieReviewService,
            MovieReviewService>();

        services.AddScoped<
            ITVShowReviewService,
            TVShowReviewService>();

        services.AddScoped<
            IEpisodeReviewService,
            EpisodeReviewService>();

        services.AddScoped<
            IMovieRecommendationService,
            MovieRecommendationService>();

        services.AddScoped<
            ITVShowRecommendationService,
            TVShowRecommendationService>();

        services.AddScoped<
            IWatchlistService,
            WatchlistService>();

        services.AddScoped<
            IWatchHistoryService,
            WatchHistoryService>();

        services.AddScoped<
            IUserProfileService,
            UserProfileService>();

        services.AddScoped<
            IGlobalSearchService,
            GlobalSearchService>();

        services.AddScoped<
            IAdminDashboardService,
            AdminDashboardService>();

        services.AddScoped<
            IAdminUserService,
            AdminUserService>();

        services.AddScoped<
            IAdminReviewService,
            AdminReviewService>();

        services.AddScoped<
            IReportService,
            ReportService>();
        services.AddScoped<
            IReportPaymentService,
            ReportPaymentService>();

        services.AddValidatorsFromAssembly(
            typeof(DependencyInjection)
                .Assembly);

        return services;
    }
}