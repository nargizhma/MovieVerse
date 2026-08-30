namespace MovieVerse.Dtos.Episodes;

public class EpisodeReturnDto
{
    public Guid Id { get; set; }

    public Guid SeasonId { get; set; }

    public string Title { get; set; } = null!;

    public int EpisodeNumber { get; set; }

    public string? Description { get; set; }

    public DateTime? ReleaseDate { get; set; }

    public int? RuntimeMinutes { get; set; }

    public string? ImageUrl { get; set; }

    public decimal? AverageRating { get; set; }

    public int ReviewCount { get; set; }
}