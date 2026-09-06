using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MovieVerse.Models;

namespace MovieVerse.Data.Configurations.People;

public class DirectorDetailConfiguration : IEntityTypeConfiguration<DirectorDetail>
{
    public void Configure(EntityTypeBuilder<DirectorDetail> builder)
    {
        builder
            .HasOne(x => x.Director)
            .WithOne(x => x.DirectorDetail)
            .HasForeignKey<DirectorDetail>(x => x.DirectorId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .Property(x => x.HeightInMeters)
            .HasPrecision(3, 2);
        builder.Property(x => x.BirthPlace)
            .HasMaxLength(200);

        builder.Property(x => x.DeathPlace)
            .HasMaxLength(200);

        builder.Property(x => x.AlternativeName)
            .HasMaxLength(200);
    }
}
