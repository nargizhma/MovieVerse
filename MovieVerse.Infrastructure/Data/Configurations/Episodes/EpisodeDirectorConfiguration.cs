using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MovieVerse.Models;

namespace MovieVerse.Data.Configurations.Episodes;

public class EpisodeDirectorConfiguration : IEntityTypeConfiguration<EpisodeDirector>
{
    public void Configure(EntityTypeBuilder<EpisodeDirector> builder)
    {
        builder
            .HasOne(x => x.Episode)
            .WithMany(x => x.EpisodeDirectors)
            .HasForeignKey(x => x.EpisodeId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne(x => x.Director)
            .WithMany()
            .HasForeignKey(x => x.DirectorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasIndex(x => new { x.EpisodeId, x.DirectorId })
            .IsUnique();
    }
}
