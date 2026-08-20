using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MovieVerse.Models;

namespace MovieVerse.Data.Configurations.TVShows;

public class TVShowReviewConfiguration : IEntityTypeConfiguration<TVShowReview>
{
    public void Configure(EntityTypeBuilder<TVShowReview> builder)
    {
        builder
            .HasOne(x => x.TVShow)
            .WithMany(x => x.Reviews)
            .HasForeignKey(x => x.TVShowId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne(x => x.User)
            .WithMany(x => x.TVShowReviews)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasIndex(x => new { x.TVShowId, x.UserId })
            .IsUnique();
    }
}
