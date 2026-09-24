using Microsoft.Extensions.Options;
using MovieVerse.Abstractions.Payments;
using Stripe;
using Stripe.Checkout;

namespace MovieVerse.Infrastructure.Payments;

public class StripeReportPaymentGateway(
    IOptions<StripeSettings> options)
    : IReportPaymentGateway
{
    private readonly StripeSettings _settings =
        options.Value;


    public async Task<PaymentCheckoutResult>
        CreateCheckoutAsync(
            Guid purchaseId,
            Guid userId,
            string productName,
            long amountInCents,
            string currency)
    {
        var frontendBaseUrl =
            _settings.FrontendBaseUrl
                .TrimEnd('/');


        var options =
            new SessionCreateOptions
            {
                Mode = "payment",

                SuccessUrl =
                    $"{frontendBaseUrl}/report-success.html"
                    +
                    "?session_id={CHECKOUT_SESSION_ID}",

                CancelUrl =
                    $"{frontendBaseUrl}/profile.html"
                    +
                    "?reportPayment=cancelled",


                LineItems =
                [
                    new SessionLineItemOptions
                    {
                        Quantity = 1,

                        PriceData =
                            new SessionLineItemPriceDataOptions
                            {
                                Currency =
                                    currency,

                                UnitAmount =
                                    amountInCents,

                                ProductData =
                                    new SessionLineItemPriceDataProductDataOptions
                                    {
                                        Name =
                                            productName,

                                        Description =
                                            "Generate a structured MovieVerse PDF report."
                                    }
                            }
                    }
                ],


                Metadata =
                    new Dictionary<string, string>
                    {
                        ["purchaseId"] =
                            purchaseId.ToString(),

                        ["userId"] =
                            userId.ToString()
                    }
            };


        var service =
            new SessionService();


        var session =
            await service.CreateAsync(
                options);


        if (
            string.IsNullOrWhiteSpace(
                session.Url))
        {
            throw new InvalidOperationException(
                "Stripe did not return a Checkout URL.");
        }


        return new PaymentCheckoutResult
        {
            SessionId =
                session.Id,

            CheckoutUrl =
                session.Url
        };
    }


    public async Task<PaymentVerificationResult>
        VerifyCheckoutAsync(
            string sessionId)
    {
        var service =
            new SessionService();


        var session =
            await service.GetAsync(
                sessionId);


        if (
            !session.Metadata.TryGetValue(
                "purchaseId",
                out var purchaseIdText)
            ||
            !Guid.TryParse(
                purchaseIdText,
                out var purchaseId))
        {
            throw new InvalidOperationException(
                "Stripe session does not contain a valid purchase ID.");
        }


        if (
            !session.Metadata.TryGetValue(
                "userId",
                out var userIdText)
            ||
            !Guid.TryParse(
                userIdText,
                out var userId))
        {
            throw new InvalidOperationException(
                "Stripe session does not contain a valid user ID.");
        }


        return new PaymentVerificationResult
        {
            IsPaid =
                string.Equals(
                    session.PaymentStatus,
                    "paid",
                    StringComparison
                        .OrdinalIgnoreCase),

            SessionId =
                session.Id,

            PaymentIntentId =
                session.PaymentIntentId,

            PurchaseId =
                purchaseId,

            UserId =
                userId
        };
    }
}