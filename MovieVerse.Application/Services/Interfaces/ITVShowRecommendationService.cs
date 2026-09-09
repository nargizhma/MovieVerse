using MovieVerse.Dtos.Recommendations;

namespace MovieVerse.Services.Interfaces;

public interface ITVShowRecommendationService
{
    Task<List<SimilarTVShowDto>>
        GetSimilarTVShowsAsync(
            Guid tvShowId,
            int limit);
}