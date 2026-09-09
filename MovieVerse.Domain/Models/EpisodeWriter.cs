using MovieVerse.Models.Common;

namespace MovieVerse.Models;

public class EpisodeWriter : BaseEntity
{
    public Guid EpisodeId { get; set; }

    public Episode Episode { get; set; } = null!;

    public Guid WriterId { get; set; }

    public Writer Writer { get; set; } = null!;
}