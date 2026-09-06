using MovieVerse.Models.Common;

namespace MovieVerse.Models;

public class MovieReview : BaseEntity
{
    public decimal Rating { get; set; }

    public string? Content { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    public Guid MovieId { get; set; }
    public Movie Movie { get; set; } = null!;

    public Guid UserId { get; set; }
    public AppUser User { get; set; } = null!;
}
