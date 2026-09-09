using MovieVerse.Models.Common;

namespace MovieVerse.Models
{
    public class MovieActor : BaseEntity
    {
        // REQUIRED
        public Guid MovieId { get; set; }

        public Movie Movie { get; set; } = null!;

        // REQUIRED
        public Guid ActorId { get; set; }

        public Actor Actor { get; set; } = null!;

        // OPTIONAL
        // Sometimes the role/character may not be entered yet
        public string? CharacterName { get; set; }
        public int CastOrder { get; set; }
    }
}