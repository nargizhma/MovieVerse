using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MovieVerse.Models;

namespace MovieVerse.Data.Configurations.People;

public class ActorConfiguration
    : IEntityTypeConfiguration<Actor>
{
    public void Configure(
        EntityTypeBuilder<Actor> builder)
    {
        builder.Property(x => x.FullName)
            .HasMaxLength(200);
    }
}