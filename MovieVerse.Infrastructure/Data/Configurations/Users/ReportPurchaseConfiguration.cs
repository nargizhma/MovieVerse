using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MovieVerse.Models;

namespace MovieVerse.Data.Configurations.Users;

public class ReportPurchaseConfiguration
    : IEntityTypeConfiguration<ReportPurchase>
{
    public void Configure(
        EntityTypeBuilder<ReportPurchase> builder)
    {
        builder
            .HasOne<AppUser>()
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);


        builder
            .Property(x => x.ReportType)
            .HasMaxLength(20)
            .IsRequired();


        builder
            .Property(x => x.PersonType)
            .HasMaxLength(20);


        builder
            .Property(x =>
                x.StripeCheckoutSessionId)
            .HasMaxLength(255);


        builder
            .Property(x =>
                x.StripePaymentIntentId)
            .HasMaxLength(255);


        builder
            .Property(x => x.Status)
            .HasMaxLength(20)
            .IsRequired();


        builder
            .Property(x => x.Currency)
            .HasMaxLength(10)
            .IsRequired();


        builder
            .HasIndex(x =>
                x.StripeCheckoutSessionId)
            .IsUnique()
            .HasFilter(
                "[StripeCheckoutSessionId] IS NOT NULL");


        builder
            .HasIndex(x => x.UserId);


        builder
            .Property(x => x.CreatedAt)
            .HasDefaultValueSql(
                "SYSUTCDATETIME()");
    }
}