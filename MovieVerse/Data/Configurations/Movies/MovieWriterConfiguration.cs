using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MovieVerse.Models;

namespace MovieVerse.Data.Configurations.Movies;

public class MovieWriterConfiguration : IEntityTypeConfiguration<MovieWriter>
{
    public void Configure(EntityTypeBuilder<MovieWriter> builder)
    {
        builder
            .HasOne(x => x.Movie)
            .WithMany(x => x.MovieWriters)
            .HasForeignKey(x => x.MovieId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne(x => x.Writer)
            .WithMany(x => x.MovieWriters)
            .HasForeignKey(x => x.WriterId)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasIndex(x => new { x.MovieId, x.WriterId })
            .IsUnique();
    }
}
