using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MovieVerse.Models;

namespace MovieVerse.Data.Configurations.Episodes;

public class EpisodeConfiguration : IEntityTypeConfiguration<Episode>
{
    public void Configure(EntityTypeBuilder<Episode> builder)
    {
        builder
            .HasOne(x => x.Season)
            .WithMany(x => x.Episodes)
            .HasForeignKey(x => x.SeasonId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasIndex(x => new { x.SeasonId, x.EpisodeNumber })
            .IsUnique();
        builder.Property(x => x.Title)
            .HasMaxLength(300);
    }
}
