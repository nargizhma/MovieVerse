using MovieVerse.Models.Common;

namespace MovieVerse.Models;

public class TVShowActor : BaseEntity
{
    public Guid TVShowId { get; set; }

    public TVShow TVShow { get; set; } = null!;

    public Guid ActorId { get; set; }

    public Actor Actor { get; set; } = null!;

    public string? CharacterName { get; set; }
}