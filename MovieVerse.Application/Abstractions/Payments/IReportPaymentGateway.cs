namespace MovieVerse.Abstractions.Payments;

public interface IReportPaymentGateway
{
    Task<PaymentCheckoutResult>
        CreateCheckoutAsync(
            Guid purchaseId,
            Guid userId,
            string productName,
            long amountInCents,
            string currency);


    Task<PaymentVerificationResult>
        VerifyCheckoutAsync(
            string sessionId);
}


public class PaymentCheckoutResult
{
    public string SessionId { get; set; }
        = null!;

    public string CheckoutUrl { get; set; }
        = null!;
}


public class PaymentVerificationResult
{
    public bool IsPaid { get; set; }

    public string SessionId { get; set; }
        = null!;

    public string? PaymentIntentId
    {
        get;
        set;
    }

    public Guid PurchaseId { get; set; }

    public Guid UserId { get; set; }
}