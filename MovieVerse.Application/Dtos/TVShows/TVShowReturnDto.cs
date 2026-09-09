using MovieVerse.Dtos.Genres;

namespace MovieVerse.Dtos.TVShows;

public class TVShowReturnDto
{
    public Guid Id { get; set; }

    public string Title { get; set; } = null!;

    public string? PosterUrl { get; set; }

    public DateTime ReleaseDate { get; set; }

    public DateTime? EndDate { get; set; }

    public string? ContentRating { get; set; }

    public int? RuntimeMinutes { get; set; }

    public decimal? AverageRating { get; set; }

    public int ReviewCount { get; set; }

    public List<GenreReturnDto> Genres { get; set; } = [];
}