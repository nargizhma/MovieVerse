using MovieVerse.Dtos.Genres;
using MovieVerse.Dtos.Seasons;

namespace MovieVerse.Dtos.TVShows;

public class TVShowDetailsDto
{
    public Guid Id { get; set; }

    public string Title { get; set; } = null!;

    public string? OriginalTitle { get; set; }

    public string? PosterUrl { get; set; }

    public string? TrailerUrl { get; set; }

    public string Synopsis { get; set; } = null!;

    public DateTime ReleaseDate { get; set; }

    public DateTime? EndDate { get; set; }

    public string? ContentRating { get; set; }

    public int? RuntimeMinutes { get; set; }


    // TVShowDetail

    public string? Storyline { get; set; }

    public string? OriginalLanguage { get; set; }

    public string? CountryOfOrigin { get; set; }

    public string? ProductionCompany { get; set; }

    public string? Trivia { get; set; }

    public string? Color { get; set; }


    // Rating

    public decimal? AverageRating { get; set; }

    public int ReviewCount { get; set; }


    // Relationships

    public List<GenreReturnDto> Genres { get; set; } = [];

    public List<TVShowCastReturnDto> Cast { get; set; } = [];

    public List<SeasonReturnDto> Seasons { get; set; } = [];
}