using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MovieVerse.Dtos.Movies;
using MovieVerse.Dtos.Recommendations;
using MovieVerse.Exceptions;
using MovieVerse.Models;
using MovieVerse.Repositories.Interfaces;
using MovieVerse.Services.Interfaces;

namespace MovieVerse.Services;

public class MovieRecommendationService(
    IGenericRepository<Movie> movieRepository,
    IMapper mapper)
    : IMovieRecommendationService
{
    public async Task<List<SimilarMovieDto>>
        GetSimilarMoviesAsync(
            Guid movieId,
            int limit)
    {
        var sourceMovie =
            await movieRepository.Query()
                .Include(x => x.MovieGenres)
                    .ThenInclude(x => x.Genre)
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    x => x.Id == movieId);

        if (sourceMovie is null)
            throw new NotFoundException(
                "Movie was not found.");

        var sourceGenreIds =
            sourceMovie.MovieGenres
                .Select(x => x.GenreId)
                .Distinct()
                .ToList();

        if (sourceGenreIds.Count == 0)
            return [];

        var candidates =
            await movieRepository.Query()
                .Where(x =>
                    x.Id != movieId &&
                    x.MovieGenres.Any(g =>
                        sourceGenreIds.Contains(
                            g.GenreId)))
                .Include(x => x.MovieGenres)
                    .ThenInclude(x => x.Genre)
                .Include(x => x.Reviews)
                .AsNoTracking()
                .ToListAsync();

        var rankedCandidates =
            candidates
                .Select(movie =>
                {
                    var sharedGenres =
                        movie.MovieGenres
                            .Where(x =>
                                sourceGenreIds.Contains(
                                    x.GenreId))
                            .Select(x =>
                                x.Genre.Name)
                            .Distinct()
                            .ToList();

                    var averageRating =
                        movie.Reviews.Count != 0
                            ? movie.Reviews
                                .Average(x =>
                                    x.Rating)
                            : 0m;

                    var genreScore =
                        sharedGenres.Count /
                        (decimal)sourceGenreIds.Count;

                    var ratingScore =
                        averageRating / 10m;

                    // Genre overlap is the main similarity signal.
                    // Community rating helps rank otherwise-similar movies.
                    var similarityScore =
                        genreScore * 0.75m +
                        ratingScore * 0.25m;

                    return new
                    {
                        Movie = movie,
                        SharedGenres =
                            sharedGenres,
                        AverageRating =
                            averageRating,
                        SimilarityScore =
                            similarityScore
                    };
                })
                .OrderByDescending(x =>
                    x.SimilarityScore)
                .ThenByDescending(x =>
                    x.SharedGenres.Count)
                .ThenByDescending(x =>
                    x.AverageRating)
                .ThenByDescending(x =>
                    x.Movie.ReleaseDate)
                .Take(limit)
                .ToList();

        return rankedCandidates
            .Select(x =>
                new SimilarMovieDto
                {
                    Movie =
                        mapper.Map<MovieReturnDto>(
                            x.Movie),

                    SharedGenreCount =
                        x.SharedGenres.Count,

                    SharedGenres =
                        x.SharedGenres,

                    SimilarityScore =
                        Math.Round(
                            x.SimilarityScore,
                            3)
                })
            .ToList();
    }
}
