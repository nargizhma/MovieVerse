using MovieVerse.Models.Common;

namespace MovieVerse.Models;

public class EpisodeReview : BaseEntity
{
    public int Rating { get; set; }

    public string? Content { get; set; }

    public Guid EpisodeId { get; set; }

    public Episode Episode { get; set; } = null!;

    public Guid UserId { get; set; }

    public AppUser User { get; set; } = null!;
}