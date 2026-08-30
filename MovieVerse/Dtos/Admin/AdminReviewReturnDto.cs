namespace MovieVerse.Dtos.Admin;

public class AdminReviewReturnDto
{
    public Guid Id { get; set; }

    // "Movie", "TVShow", or "Episode"
    public string ContentType { get; set; } = null!;

    public Guid ContentId { get; set; }

    public string Title { get; set; } = null!;

    public Guid UserId { get; set; }

    public string UserName { get; set; } = null!;

    public string? DisplayName { get; set; }

    public decimal Rating { get; set; }

    public string? Content { get; set; }
}
