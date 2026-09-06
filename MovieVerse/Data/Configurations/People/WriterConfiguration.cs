using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MovieVerse.Models;

namespace MovieVerse.Data.Configurations.People;

public class WriterConfiguration
    : IEntityTypeConfiguration<Writer>
{
    public void Configure(
        EntityTypeBuilder<Writer> builder)
    {
        builder.Property(x => x.FullName)
            .HasMaxLength(200);
    }
}