using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MovieVerse.Models;

namespace MovieVerse.Data.Configurations.TVShows;

public class TVShowConfiguration
    : IEntityTypeConfiguration<TVShow>
{
    public void Configure(
        EntityTypeBuilder<TVShow> builder)
    {
        builder.Property(x => x.Title)
            .HasMaxLength(300);

        builder.Property(x => x.OriginalTitle)
            .HasMaxLength(300);
    }
}