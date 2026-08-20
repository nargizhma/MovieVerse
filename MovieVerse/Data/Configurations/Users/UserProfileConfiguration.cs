using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MovieVerse.Models;

namespace MovieVerse.Data.Configurations.Users;

public class UserProfileConfiguration : IEntityTypeConfiguration<UserProfile>
{
    public void Configure(EntityTypeBuilder<UserProfile> builder)
    {
        builder.HasKey(x => x.AppUserId);

        builder
            .HasOne(x => x.AppUser)
            .WithOne(x => x.Profile)
            .HasForeignKey<UserProfile>(x => x.AppUserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(x => x.DisplayName)
            .HasMaxLength(50);

        builder.Property(x => x.Bio)
            .HasMaxLength(500);

        builder.Property(x => x.ProfileImageUrl)
            .HasMaxLength(500);
    }
}