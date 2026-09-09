using MovieVerse.Models.Common;

namespace MovieVerse.Models;

public class Writer : BaseEntity
{
    public string FullName { get; set; } = null!;

    public string? ProfileImageUrl { get; set; }

    public WriterDetail? WriterDetail { get; set; }

    public List<MovieWriter> MovieWriters { get; set; } = [];
}