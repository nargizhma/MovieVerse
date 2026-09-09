namespace MovieVerse.Dtos.Seasons;

public class SeasonReturnDto
{
    public Guid Id { get; set; }

    public int SeasonNumber { get; set; }

    public Guid TVShowId { get; set; }

    public int EpisodeCount { get; set; }
}