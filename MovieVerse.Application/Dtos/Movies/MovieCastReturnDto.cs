namespace MovieVerse.Dtos.Movies;

public class MovieCastReturnDto
{
    public Guid ActorId { get; set; }

    public string FullName { get; set; } = null!;

    public string? CharacterName { get; set; }

    public int CastOrder { get; set; }
}