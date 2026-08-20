using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MovieVerse.Models;

namespace MovieVerse.Data.Configurations.TVShows;

public class SeasonConfiguration : IEntityTypeConfiguration<Season>
{
    public void Configure(EntityTypeBuilder<Season> builder)
    {
        builder
            .HasOne(x => x.TVShow)
            .WithMany(x => x.Seasons)
            .HasForeignKey(x => x.TVShowId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasIndex(x => new { x.TVShowId, x.SeasonNumber })
            .IsUnique();
    }
}
