using MovieVerse.Models.Common;

namespace MovieVerse.Models;

public class TVShow : BaseEntity
{
    public string Title { get; set; } = null!;

    public string? OriginalTitle { get; set; }

    public string? PosterUrl { get; set; }

    public string? TrailerUrl { get; set; }

    public string Synopsis { get; set; } = null!;

    public DateTime ReleaseDate { get; set; }

    // Null if the series is still ongoing
    public DateTime? EndDate { get; set; }

    // TV-14, TV-MA, etc.
    public string? ContentRating { get; set; }

    // Typical episode runtime
    public int? RuntimeMinutes { get; set; }

    public TVShowDetail? TVShowDetail { get; set; }

    public List<Season> Seasons { get; set; } = [];

    public List<TVShowGenre> TVShowGenres { get; set; } = [];

    public List<TVShowActor> TVShowActors { get; set; } = [];

    public List<TVShowReview> Reviews { get; set; } = [];
}