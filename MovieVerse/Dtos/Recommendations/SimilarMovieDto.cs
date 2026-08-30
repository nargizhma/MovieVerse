using MovieVerse.Dtos.Movies;

namespace MovieVerse.Dtos.Recommendations;

public class SimilarMovieDto
{
    public MovieReturnDto Movie { get; set; } = null!;

    public int SharedGenreCount { get; set; }

    public List<string> SharedGenres { get; set; } = [];

    // 0.000 - 1.000
    public decimal SimilarityScore { get; set; }
}
