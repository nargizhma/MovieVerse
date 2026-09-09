using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MovieVerse.Models;

namespace MovieVerse.Data.Configurations.Users;

public class WatchHistoryItemConfiguration
    : IEntityTypeConfiguration<WatchHistoryItem>
{
    public void Configure(
        EntityTypeBuilder<WatchHistoryItem> builder)
    {
        builder
            .HasOne<AppUser>()
            .WithMany(x => x.WatchHistoryItems)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne(x => x.Movie)
            .WithMany()
            .HasForeignKey(x => x.MovieId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne(x => x.TVShow)
            .WithMany()
            .HasForeignKey(x => x.TVShowId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasIndex(x =>
                new
                {
                    x.UserId,
                    x.MovieId
                })
            .IsUnique()
            .HasFilter(
                "[MovieId] IS NOT NULL");

        builder
            .HasIndex(x =>
                new
                {
                    x.UserId,
                    x.TVShowId
                })
            .IsUnique()
            .HasFilter(
                "[TVShowId] IS NOT NULL");

        builder.Property(x => x.WatchedAt)
            .HasDefaultValueSql(
                "SYSUTCDATETIME()");

        builder.ToTable(t =>
            t.HasCheckConstraint(
                "CK_WatchHistoryItem_Content",
                "([MovieId] IS NOT NULL AND [TVShowId] IS NULL) OR " +
                "([MovieId] IS NULL AND [TVShowId] IS NOT NULL)"
            ));
    }
}