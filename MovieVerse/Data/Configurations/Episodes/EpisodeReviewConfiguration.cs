using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MovieVerse.Models;

namespace MovieVerse.Data.Configurations.Episodes;

public class EpisodeReviewConfiguration : IEntityTypeConfiguration<EpisodeReview>
{
    public void Configure(EntityTypeBuilder<EpisodeReview> builder)
    {
        builder
            .HasOne(x => x.Episode)
            .WithMany(x => x.Reviews)
            .HasForeignKey(x => x.EpisodeId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne(x => x.User)
            .WithMany(x => x.EpisodeReviews)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasIndex(x => new { x.EpisodeId, x.UserId })
            .IsUnique();
        builder.Property(x => x.Rating)
            .HasPrecision(3, 1);
    }
}
