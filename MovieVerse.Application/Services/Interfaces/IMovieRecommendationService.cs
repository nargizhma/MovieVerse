using MovieVerse.Dtos.Recommendations;

namespace MovieVerse.Services.Interfaces;

public interface IMovieRecommendationService
{
    Task<List<SimilarMovieDto>> GetSimilarMoviesAsync(
        Guid movieId,
        int limit);
}
