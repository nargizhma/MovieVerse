using MovieVerse.Models.Common;

namespace MovieVerse.Models;

public class EpisodeReview : BaseEntity
{
    public decimal Rating { get; set; }

    public string? Content { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    public Guid EpisodeId { get; set; }
    public Episode Episode { get; set; } = null!;

    public Guid UserId { get; set; }
    public AppUser User { get; set; } = null!;
}
