using MovieVerse.Models.Common;

namespace MovieVerse.Models
{
    public class MovieDirector : BaseEntity
    {
        public Guid MovieId { get; set; }
        public Movie Movie { get; set; } = null!;

        public Guid DirectorId { get; set; }
        public Director Director { get; set; } = null!;
    }
}