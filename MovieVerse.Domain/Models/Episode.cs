using MovieVerse.Models.Common;

namespace MovieVerse.Models;

public class Episode : BaseEntity
{
    public string Title { get; set; } = null!;

    public int EpisodeNumber { get; set; }

    public string? Description { get; set; }

    public DateTime? ReleaseDate { get; set; }

    public int? RuntimeMinutes { get; set; }

    public string? ImageUrl { get; set; }

    public Guid SeasonId { get; set; }

    public Season Season { get; set; } = null!;

    public List<EpisodeReview> Reviews { get; set; } = [];

    public List<EpisodeActor> EpisodeActors { get; set; } = [];

    public List<EpisodeDirector> EpisodeDirectors { get; set; } = [];

    public List<EpisodeWriter> EpisodeWriters { get; set; } = [];
}