using MovieVerse.Models.Common;

namespace MovieVerse.Models;

public class TVShowGenre : BaseEntity
{
    public Guid TVShowId { get; set; }

    public TVShow TVShow { get; set; } = null!;

    public Guid GenreId { get; set; }

    public Genre Genre { get; set; } = null!;
}