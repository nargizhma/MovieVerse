using MovieVerse.Abstractions.Media;
using MovieVerse.Dtos.Episodes;

namespace MovieVerse.Requests.Episodes;

public class EpisodeUpdateRequest
{
    public string Title { get; set; } = null!;

    public int EpisodeNumber { get; set; }

    public string? Description { get; set; }

    public DateTime? ReleaseDate { get; set; }

    public int? RuntimeMinutes { get; set; }

    public UploadedFile? Image { get; set; }

    public List<EpisodeActorInputDto> Actors
    { get; set; } = [];

    public List<Guid> DirectorIds
    { get; set; } = [];

    public List<Guid> WriterIds
    { get; set; } = [];
}