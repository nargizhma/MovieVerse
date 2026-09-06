using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MovieVerse.Models;

namespace MovieVerse.Data.Configurations.Movies;

public class MovieActorConfiguration : IEntityTypeConfiguration<MovieActor>
{
    public void Configure(EntityTypeBuilder<MovieActor> builder)
    {
        builder
            .HasOne(x => x.Movie)
            .WithMany(x => x.MovieActors)
            .HasForeignKey(x => x.MovieId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne(x => x.Actor)
            .WithMany(x => x.MovieActors)
            .HasForeignKey(x => x.ActorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasIndex(x => new { x.MovieId, x.ActorId })
            .IsUnique();
        builder.Property(x => x.CharacterName)
            .HasMaxLength(200);
    }
}
