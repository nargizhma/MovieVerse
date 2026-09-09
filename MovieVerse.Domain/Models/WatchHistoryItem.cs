using MovieVerse.Models.Common;

namespace MovieVerse.Models;

public class WatchHistoryItem : BaseEntity
{
    public Guid UserId { get; set; }

    public Guid? MovieId { get; set; }
    public Movie? Movie { get; set; }

    public Guid? TVShowId { get; set; }
    public TVShow? TVShow { get; set; }

    public DateTime WatchedAt { get; set; }
        = DateTime.UtcNow;
}