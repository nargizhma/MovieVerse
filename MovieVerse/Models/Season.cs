using MovieVerse.Models.Common;

namespace MovieVerse.Models;

public class Season : BaseEntity
{
    public int SeasonNumber { get; set; }

    public Guid TVShowId { get; set; }

    public TVShow TVShow { get; set; } = null!;

    public List<Episode> Episodes { get; set; } = [];
}