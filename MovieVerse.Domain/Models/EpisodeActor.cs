using MovieVerse.Models.Common;

namespace MovieVerse.Models;

public class EpisodeActor : BaseEntity
{
    public Guid EpisodeId { get; set; }

    public Episode Episode { get; set; } = null!;

    public Guid ActorId { get; set; }

    public Actor Actor { get; set; } = null!;

    public string? CharacterName { get; set; }

    public int CastOrder { get; set; }
}