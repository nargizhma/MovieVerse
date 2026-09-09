using MovieVerse.Abstractions.Media;
using MovieVerse.Dtos.Movies;

namespace MovieVerse.Requests.Movies;

public class MovieCreateRequest
{
    public string Title { get; set; } = null!;
    public UploadedFile? PosterImage { get; set; }
    public string? TrailerUrl { get; set; }
    public DateTime ReleaseDate { get; set; }
    public string? ContentRating { get; set; }
    public int RuntimeMinutes { get; set; }
    public string Synopsis { get; set; } = null!;

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

    public List<Guid> GenreIds { get; set; } = [];
    public List<MovieActorInputDto> Actors { get; set; } = [];
    public List<Guid> DirectorIds { get; set; } = [];
    public List<Guid> WriterIds { get; set; } = [];
}