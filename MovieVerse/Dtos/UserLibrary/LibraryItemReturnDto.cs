namespace MovieVerse.Dtos.UserLibrary;

public class LibraryItemReturnDto
{
    public Guid Id { get; set; }

    public string ContentType { get; set; } = null!;

    public Guid ContentId { get; set; }

    public string Title { get; set; } = null!;

    public string? PosterUrl { get; set; }

    public DateTime ReleaseDate { get; set; }

    public decimal? AverageRating { get; set; }

    public DateTime ActivityAt { get; set; }
}
