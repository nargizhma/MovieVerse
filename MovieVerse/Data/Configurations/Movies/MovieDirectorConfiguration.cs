using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MovieVerse.Models;

namespace MovieVerse.Data.Configurations.Movies;

public class MovieDirectorConfiguration : IEntityTypeConfiguration<MovieDirector>
{
    public void Configure(EntityTypeBuilder<MovieDirector> builder)
    {
        builder
            .HasOne(x => x.Movie)
            .WithMany(x => x.MovieDirectors)
            .HasForeignKey(x => x.MovieId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne(x => x.Director)
            .WithMany(x => x.MovieDirectors)
            .HasForeignKey(x => x.DirectorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasIndex(x => new { x.MovieId, x.DirectorId })
            .IsUnique();
    }
}
