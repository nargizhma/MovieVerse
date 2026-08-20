using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MovieVerse.Models;

namespace MovieVerse.Data.Configurations.People;

public class ActorDetailConfiguration : IEntityTypeConfiguration<ActorDetail>
{
    public void Configure(EntityTypeBuilder<ActorDetail> builder)
    {
        builder
            .HasOne(x => x.Actor)
            .WithOne(x => x.ActorDetail)
            .HasForeignKey<ActorDetail>(x => x.ActorId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .Property(x => x.HeightInMeters)
            .HasPrecision(3, 2);
    }
}
