using MovieVerse.Models.Common;

namespace MovieVerse.Models;

public class Director : BaseEntity
{
    public string FullName { get; set; } = null!;

    public string? ProfileImageUrl { get; set; }

    public DirectorDetail? DirectorDetail { get; set; }

    public List<MovieDirector> MovieDirectors { get; set; } = [];
}