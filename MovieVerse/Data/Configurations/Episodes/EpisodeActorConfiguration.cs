using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MovieVerse.Models;

namespace MovieVerse.Data.Configurations.Episodes;

public class EpisodeActorConfiguration : IEntityTypeConfiguration<EpisodeActor>
{
    public void Configure(EntityTypeBuilder<EpisodeActor> builder)
    {
        builder
            .HasOne(x => x.Episode)
            .WithMany(x => x.EpisodeActors)
            .HasForeignKey(x => x.EpisodeId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne(x => x.Actor)
            .WithMany()
            .HasForeignKey(x => x.ActorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasIndex(x => new { x.EpisodeId, x.ActorId })
            .IsUnique();
    }
}
