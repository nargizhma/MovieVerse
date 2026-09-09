using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MovieVerse.Models;

namespace MovieVerse.Data.Configurations.People;

public class DirectorConfiguration
    : IEntityTypeConfiguration<Director>
{
    public void Configure(
        EntityTypeBuilder<Director> builder)
    {
        builder.Property(x => x.FullName)
            .HasMaxLength(200);
    }
}