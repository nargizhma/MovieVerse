namespace MovieVerse.Infrastructure.Payments;

public class StripeSettings
{
    public const string SectionName =
        "Stripe";


    public string SecretKey { get; set; }
        = string.Empty;


    public string FrontendBaseUrl { get; set; } = "http://127.0.0.1:5500";


    public long ReportPriceInCents { get; set; } = 99;


    public string Currency { get; set; }
        = "usd";
}