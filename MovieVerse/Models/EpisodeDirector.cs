using MovieVerse.Models.Common;

namespace MovieVerse.Models;

public class EpisodeDirector : BaseEntity
{
    public Guid EpisodeId { get; set; }

    public Episode Episode { get; set; } = null!;

    public Guid DirectorId { get; set; }

    public Director Director { get; set; } = null!;
}