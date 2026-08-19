using MovieVerse.Models.Common;

namespace MovieVerse.Models;

public class TVShowReview : BaseEntity
{
    public int Rating { get; set; }

    public string? Content { get; set; }

    public Guid TVShowId { get; set; }

    public TVShow TVShow { get; set; } = null!;

    public Guid UserId { get; set; }

    public AppUser User { get; set; } = null!;
}