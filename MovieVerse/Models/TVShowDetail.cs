using MovieVerse.Models.Common;

namespace MovieVerse.Models;

public class TVShowDetail : BaseEntity
{
    public string? Storyline { get; set; }

    public string? OriginalLanguage { get; set; }

    public string? CountryOfOrigin { get; set; }

    public string? ProductionCompany { get; set; }

    public string? Trivia { get; set; }

    public string? Color { get; set; }

    public Guid TVShowId { get; set; }

    public TVShow TVShow { get; set; } = null!;
}