using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MovieVerse.Models;

namespace MovieVerse.Data.Configurations.TVShows;

public class TVShowGenreConfiguration : IEntityTypeConfiguration<TVShowGenre>
{
    public void Configure(EntityTypeBuilder<TVShowGenre> builder)
    {
        builder
            .HasOne(x => x.TVShow)
            .WithMany(x => x.TVShowGenres)
            .HasForeignKey(x => x.TVShowId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne(x => x.Genre)
            .WithMany(x => x.TVShowGenres)
            .HasForeignKey(x => x.GenreId)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasIndex(x => new { x.TVShowId, x.GenreId })
            .IsUnique();
    }
}
