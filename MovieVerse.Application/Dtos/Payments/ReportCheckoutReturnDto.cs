namespace MovieVerse.Dtos.Payments;

public class ReportCheckoutReturnDto
{
    public Guid PurchaseId { get; set; }

    public string CheckoutUrl { get; set; }
        = null!;
}