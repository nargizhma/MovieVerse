using MovieVerse.Dtos.Genres;

namespace MovieVerse.Dtos.Movies;

public class MovieDetailsDto
{
    public Guid Id { get; set; }

    public string Title { get; set; } = null!;

    public string? PosterUrl { get; set; }

    public string? TrailerUrl { get; set; }

    public DateTime ReleaseDate { get; set; }

    public string? ContentRating { get; set; }

    public int RuntimeMinutes { get; set; }

    public string Synopsis { get; set; } = null!;


    // MovieDetail

    public string? Storyline { get; set; }

    public string? Tagline { get; set; }

    public string? OriginalLanguage { get; set; }

    public string? CountryOfOrigin { get; set; }

    public string? FilmingLocation { get; set; }

    public string? ProductionCompany { get; set; }

    public decimal? Budget { get; set; }

    public decimal? GrossWorldwide { get; set; }

    public string? Color { get; set; }

    public string? SoundMix { get; set; }

    public string? Trivia { get; set; }


    // Rating information

    public decimal? AverageRating { get; set; }

    public int ReviewCount { get; set; }


    // Relationships

    public List<GenreReturnDto> Genres { get; set; } = [];

    public List<MovieCastReturnDto> Cast { get; set; } = [];

    public List<MovieCrewReturnDto> Directors { get; set; } = [];

    public List<MovieCrewReturnDto> Writers { get; set; } = [];
}