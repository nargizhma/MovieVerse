namespace MovieVerse.Dtos.TVShows;

public class TVShowActorInputDto
{
    public Guid ActorId { get; set; }

    public string? CharacterName { get; set; }

    public int CastOrder { get; set; }
}