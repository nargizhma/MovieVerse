using MovieVerse.Dtos.TVShows;

namespace MovieVerse.Dtos.Recommendations;

public class SimilarTVShowDto
{
    public TVShowReturnDto TVShow { get; set; } = null!;

    public int SharedGenreCount { get; set; }

    public List<string> SharedGenres { get; set; } = [];

    public decimal SimilarityScore { get; set; }
}