using MovieVerse.Abstractions.Media;
using MovieVerse.Dtos.TVShows;

namespace MovieVerse.Requests.TVShows;

public class TVShowUpdateRequest
{
    public string Title { get; set; } = null!;
    public string? OriginalTitle { get; set; }
    public UploadedFile? PosterImage { get; set; }
    public string? TrailerUrl { get; set; }
    public string Synopsis { get; set; } = null!;
    public DateTime ReleaseDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string? ContentRating { get; set; }
    public int? RuntimeMinutes { get; set; }

    public string? Storyline { get; set; }
    public string? OriginalLanguage { get; set; }
    public string? CountryOfOrigin { get; set; }
    public string? ProductionCompany { get; set; }
    public string? Trivia { get; set; }
    public string? Color { get; set; }

    public List<Guid> GenreIds { get; set; } = [];
    public List<TVShowActorInputDto> Actors { get; set; } = [];
}