using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MovieVerse.Models;

namespace MovieVerse.Data.Configurations.People;

public class WriterDetailConfiguration : IEntityTypeConfiguration<WriterDetail>
{
    public void Configure(EntityTypeBuilder<WriterDetail> builder)
    {
        builder
            .HasOne(x => x.Writer)
            .WithOne(x => x.WriterDetail)
            .HasForeignKey<WriterDetail>(x => x.WriterId)
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
