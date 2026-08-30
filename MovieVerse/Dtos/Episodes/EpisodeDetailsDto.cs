namespace MovieVerse.Dtos.Episodes;

public class EpisodeDetailsDto
{
    public Guid Id { get; set; }

    public Guid SeasonId { get; set; }

    public int SeasonNumber { get; set; }

    public Guid TVShowId { get; set; }

    public string TVShowTitle { get; set; } = null!;

    public string Title { get; set; } = null!;

    public int EpisodeNumber { get; set; }

    public string? Description { get; set; }

    public DateTime? ReleaseDate { get; set; }

    public int? RuntimeMinutes { get; set; }

    public string? ImageUrl { get; set; }

    public decimal? AverageRating { get; set; }

    public int ReviewCount { get; set; }


    // Relationships

    public List<EpisodeCastReturnDto> Cast
    { get; set; } = [];

    public List<EpisodeCrewReturnDto> Directors
    { get; set; } = [];

    public List<EpisodeCrewReturnDto> Writers
    { get; set; } = [];
}