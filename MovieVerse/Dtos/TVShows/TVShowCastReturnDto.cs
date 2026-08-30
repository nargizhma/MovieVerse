namespace MovieVerse.Dtos.TVShows;

public class TVShowCastReturnDto
{
    public Guid ActorId { get; set; }

    public string FullName { get; set; } = null!;

    public string? CharacterName { get; set; }

    public int CastOrder { get; set; }
}