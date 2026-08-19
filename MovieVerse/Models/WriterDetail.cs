using MovieVerse.Models.Common;

namespace MovieVerse.Models
{
    public class WriterDetail : BaseEntity
    {
        public string? Biography { get; set; }

        public DateTime? BirthDate { get; set; }

        public string? BirthPlace { get; set; }

        public DateTime? DeathDate { get; set; }

        public string? DeathPlace { get; set; }

        public decimal? HeightInMeters { get; set; }

        public string? AlternativeName { get; set; }

        public string? Nickname { get; set; }

        public string? Spouse { get; set; }

        public string? Children { get; set; }

        public string? Parents { get; set; }

        public string? Relatives { get; set; }

        public string? OtherWorks { get; set; }

        public string? Trivia { get; set; }

        public string? Quote { get; set; }

        public string? Trademark { get; set; }

        public Guid WriterId { get; set; }

        public Writer Writer { get; set; } = null!;
    }
}