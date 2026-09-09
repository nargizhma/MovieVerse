using MovieVerse.Models.Common;

namespace MovieVerse.Models
{
    public class MovieWriter : BaseEntity
    {
        public Guid MovieId { get; set; }

        public Movie Movie { get; set; } = null!;

        public Guid WriterId { get; set; }

        public Writer Writer { get; set; } = null!;
    }
}