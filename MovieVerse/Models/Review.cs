using MovieVerse.Models.Common;

namespace MovieVerse.Models
{
    public class Review : BaseEntity
    {
        public int Rating { get; set; }

        public string? Content { get; set; }

        public Guid MovieId { get; set; }
        public Movie Movie { get; set; } = null!;

        public Guid UserId { get; set; }
        public AppUser User { get; set; } = null!;
    }
}