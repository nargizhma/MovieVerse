namespace MovieVerse.Dtos.Movies;

public class MovieActorInputDto
{
    public Guid ActorId { get; set; }

    public string? CharacterName { get; set; }

    public int CastOrder { get; set; }
}