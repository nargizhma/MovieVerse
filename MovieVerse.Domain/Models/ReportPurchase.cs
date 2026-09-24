using MovieVerse.Models.Common;

namespace MovieVerse.Models;

public class ReportPurchase : BaseEntity
{
    public Guid UserId { get; set; }
    public string ReportType { get; set; }
        = null!;
    public string? PersonType { get; set; }
    public Guid? PersonId { get; set; }

    public string? StripeCheckoutSessionId { get; set; }

    public string? StripePaymentIntentId { get; set; }


    public string Status { get; set; }
        = "Pending";


    public long AmountInCents { get; set; }

    public string Currency { get; set; }
        = "usd";


    public DateTime CreatedAt { get; set; }
        = DateTime.UtcNow;

    public DateTime? PaidAt { get; set; }
}