using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MovieVerse.Models;

namespace MovieVerse.Data.Configurations.Episodes;

public class EpisodeWriterConfiguration : IEntityTypeConfiguration<EpisodeWriter>
{
    public void Configure(EntityTypeBuilder<EpisodeWriter> builder)
    {
        builder
            .HasOne(x => x.Episode)
            .WithMany(x => x.EpisodeWriters)
            .HasForeignKey(x => x.EpisodeId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne(x => x.Writer)
            .WithMany()
            .HasForeignKey(x => x.WriterId)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasIndex(x => new { x.EpisodeId, x.WriterId })
            .IsUnique();
    }
}
