namespace MovieVerse.Dtos.Profiles;

public class ProfileActivityItemDto
{
    public Guid ReviewId { get; set; }

    public string ContentType { get; set; } = null!;

    public Guid ContentId { get; set; }

    public Guid? TVShowId { get; set; }

    public string Title { get; set; } = null!;

    public string? ParentTitle { get; set; }

    public int? SeasonNumber { get; set; }

    public int? EpisodeNumber { get; set; }

    public string? ImageUrl { get; set; }

    public decimal Rating { get; set; }

    public string? Content { get; set; }
}