using MovieVerse.Models.Common;

namespace MovieVerse.Models
{
    public class Genre : BaseEntity
    {
        public string Name { get; set; } = null!;

        public List<MovieGenre> MovieGenres { get; set; } = [];

        public List<TVShowGenre> TVShowGenres { get; set; } = [];
    }
}