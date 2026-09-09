using AutoMapper;
using MovieVerse.Dtos.Recommendations;
using MovieVerse.Dtos.TVShows;
using MovieVerse.Exceptions;
using MovieVerse.Models;
using MovieVerse.Repositories.Interfaces;
using MovieVerse.Services.Interfaces;

namespace MovieVerse.Services;

public class TVShowRecommendationService(
    IGenericRepository<TVShow> tvShowRepository,
    IMapper mapper)
    : ITVShowRecommendationService
{
    public async Task<List<SimilarTVShowDto>>
        GetSimilarTVShowsAsync(
            Guid tvShowId,
            int limit)
    {
        var sourceTVShow =
            await tvShowRepository
                .FirstOrDefaultAsync(
                    x => x.Id == tvShowId,
                    false,
                    "TVShowGenres.Genre");

        if (sourceTVShow is null)
            throw new NotFoundException(
                "TV show was not found.");

        var sourceGenreIds =
            sourceTVShow.TVShowGenres
                .Select(x => x.GenreId)
                .Distinct()
                .ToList();

        if (sourceGenreIds.Count == 0)
            return [];

        var candidates =
            await tvShowRepository.FindAllAsync(
                x =>
                    x.Id != tvShowId &&
                    x.TVShowGenres.Any(g =>
                        sourceGenreIds.Contains(
                            g.GenreId)),
                false,
                "TVShowGenres.Genre",
                "Reviews");

        var rankedCandidates =
            candidates
                .Select(tvShow =>
                {
                    var sharedGenres =
                        tvShow.TVShowGenres
                            .Where(x =>
                                sourceGenreIds.Contains(
                                    x.GenreId))
                            .Select(x =>
                                x.Genre.Name)
                            .Distinct()
                            .ToList();

                    var averageRating =
                        tvShow.Reviews.Count != 0
                            ? tvShow.Reviews
                                .Average(x => x.Rating)
                            : 0m;

                    var genreScore =
                        sharedGenres.Count /
                        (decimal)sourceGenreIds.Count;

                    var ratingScore =
                        averageRating / 10m;

                    var similarityScore =
                        genreScore * 0.75m +
                        ratingScore * 0.25m;

                    return new
                    {
                        TVShow = tvShow,
                        SharedGenres = sharedGenres,
                        AverageRating = averageRating,
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
                    x.TVShow.ReleaseDate)
                .Take(limit)
                .ToList();

        return rankedCandidates
            .Select(x =>
                new SimilarTVShowDto
                {
                    TVShow =
                        mapper.Map<TVShowReturnDto>(
                            x.TVShow),

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