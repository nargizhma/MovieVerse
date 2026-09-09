using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MovieVerse.Models;

namespace MovieVerse.Data.Configurations.Movies;

public class MovieConfiguration
    : IEntityTypeConfiguration<Movie>
{
    public void Configure(
        EntityTypeBuilder<Movie> builder)
    {
        builder.Property(x => x.Title)
            .HasMaxLength(300);
    }
}