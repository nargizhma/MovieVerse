namespace MovieVerse.Dtos.People;

public class FilmographyItemDto
{
    public Guid Id { get; set; }

    // "Movie" or "TVShow"
    public string ContentType { get; set; } = null!;

    public string Title { get; set; } = null!;

    public int ReleaseYear { get; set; }

    public string? PosterUrl { get; set; }

    // Used for actors. Null for directors.
    public string? CharacterName { get; set; }

    // For TV series credits that come from episode-level credits.
    public int? EpisodeCount { get; set; }
}
