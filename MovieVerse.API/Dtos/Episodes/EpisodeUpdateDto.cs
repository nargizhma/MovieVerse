namespace MovieVerse.Dtos.Episodes;

public class EpisodeUpdateDto
{
    public string Title { get; set; } = null!;

    public int EpisodeNumber { get; set; }

    public string? Description { get; set; }

    public DateTime? ReleaseDate { get; set; }

    public int? RuntimeMinutes { get; set; }

    public IFormFile? Image { get; set; }


    // Relationships

    public List<EpisodeActorInputDto> Actors
    { get; set; } = [];

    public List<Guid> DirectorIds
    { get; set; } = [];

    public List<Guid> WriterIds
    { get; set; } = [];
}