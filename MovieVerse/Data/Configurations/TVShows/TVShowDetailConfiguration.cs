using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MovieVerse.Models;

namespace MovieVerse.Data.Configurations.TVShows;

public class TVShowDetailConfiguration : IEntityTypeConfiguration<TVShowDetail>
{
    public void Configure(EntityTypeBuilder<TVShowDetail> builder)
    {
        builder
            .HasOne(x => x.TVShow)
            .WithOne(x => x.TVShowDetail)
            .HasForeignKey<TVShowDetail>(x => x.TVShowId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
