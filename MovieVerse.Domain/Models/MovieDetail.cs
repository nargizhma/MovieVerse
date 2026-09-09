using MovieVerse.Models.Common;

namespace MovieVerse.Models
{
    public class MovieDetail : BaseEntity
    {
        public string? Storyline { get; set; }
        public string? Tagline { get; set; }

        public string? OriginalLanguage { get; set; }

        public string? CountryOfOrigin { get; set; }

        public string? FilmingLocation { get; set; }

        public string? ProductionCompany { get; set; }

        public decimal? Budget { get; set; }

        public decimal? GrossWorldwide { get; set; }

        public string? Color { get; set; }

        public string? SoundMix { get; set; }

        // IMDb "Did you know?" type information
        public string? Trivia { get; set; }

        // FK
        public Guid MovieId { get; set; }
        public Movie Movie { get; set; } = null!;
    }
}