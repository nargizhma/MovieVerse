namespace MovieVerse.Dtos.Episodes;

public class EpisodeActorInputDto
{
    public Guid ActorId { get; set; }

    public string? CharacterName { get; set; }

    public int CastOrder { get; set; }
}