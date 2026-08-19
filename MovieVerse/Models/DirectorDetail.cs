using MovieVerse.Models.Common;

namespace MovieVerse.Models
{
    public class DirectorDetail : BaseEntity
    {
        public string? Biography { get; set; }

        public DateTime? BirthDate { get; set; }

        public string? BirthPlace { get; set; }

        public string? AlternativeName { get; set; }

        public string? Nickname { get; set; }

        public decimal? HeightInMeters { get; set; }

        public string? Trivia { get; set; }

        public string? Quote { get; set; }

        public Guid DirectorId { get; set; }

        public Director Director { get; set; } = null!;
    }
}