using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MovieVerse.Models;

namespace MovieVerse.Data.Configurations.TVShows;

public class TVShowActorConfiguration : IEntityTypeConfiguration<TVShowActor>
{
    public void Configure(EntityTypeBuilder<TVShowActor> builder)
    {
        builder
            .HasOne(x => x.TVShow)
            .WithMany(x => x.TVShowActors)
            .HasForeignKey(x => x.TVShowId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne(x => x.Actor)
            .WithMany()
            .HasForeignKey(x => x.ActorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasIndex(x => new { x.TVShowId, x.ActorId })
            .IsUnique();
        builder.Property(x => x.CharacterName)
            .HasMaxLength(200);
    }
}
