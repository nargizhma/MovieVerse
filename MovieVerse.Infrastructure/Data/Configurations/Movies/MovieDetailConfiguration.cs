using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MovieVerse.Models;

namespace MovieVerse.Data.Configurations.Movies;

public class MovieDetailConfiguration : IEntityTypeConfiguration<MovieDetail>
{
    public void Configure(EntityTypeBuilder<MovieDetail> builder)
    {
        builder
            .HasOne(x => x.Movie)
            .WithOne(x => x.MovieDetail)
            .HasForeignKey<MovieDetail>(x => x.MovieId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .Property(x => x.Budget)
            .HasPrecision(18, 2);

        builder
            .Property(x => x.GrossWorldwide)
            .HasPrecision(18, 2);
    }
}
