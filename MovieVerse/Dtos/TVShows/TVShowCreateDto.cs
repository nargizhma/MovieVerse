namespace MovieVerse.Dtos.TVShows;

public class TVShowCreateDto
{
    public string Title { get; set; } = null!;

    public string? OriginalTitle { get; set; }

    public IFormFile? PosterImage { get; set; }

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


    // Relationships

    public List<Guid> GenreIds { get; set; } = [];

    public List<TVShowActorInputDto> Actors { get; set; } = [];
}