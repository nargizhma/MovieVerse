using MovieVerse.Models.Common;

namespace MovieVerse.Models;

public class Movie : BaseEntity
{
    public string Title { get; set; } = null!;

    public string? PosterUrl { get; set; }
    public string? TrailerUrl { get; set; }

    public DateTime ReleaseDate { get; set; }

    public string? ContentRating { get; set; }

    public int RuntimeMinutes { get; set; }

    public string Synopsis { get; set; } = null!;

    public MovieDetail? MovieDetail { get; set; }

    public List<MovieDirector> MovieDirectors { get; set; } = [];
    public List<MovieGenre> MovieGenres { get; set; } = [];
    public List<MovieActor> MovieActors { get; set; } = [];
    public List<Review> Reviews { get; set; } = [];
    public List<MovieWriter> MovieWriters { get; set; } = [];
}