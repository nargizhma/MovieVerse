using MovieVerse.Abstractions.Payments;
using MovieVerse.Abstractions.Persistence;
using MovieVerse.Dtos.Payments;
using MovieVerse.Exceptions;
using MovieVerse.Models;
using MovieVerse.Repositories.Interfaces;
using MovieVerse.Services.Interfaces;

namespace MovieVerse.Services;

public class ReportPaymentService(
    IGenericRepository<ReportPurchase>
        purchaseRepository,
    IUnitOfWork unitOfWork,
    IReportPaymentGateway paymentGateway)
    : IReportPaymentService
{
    private const long ReportPrice =
        99;

    private const string Currency =
        "usd";


    public async Task<ReportCheckoutReturnDto>
        CreateCheckoutAsync(
            Guid userId,
            CreateReportCheckoutDto dto)
    {
        var reportType =
            NormalizeReportType(
                dto.ReportType);


        string? personType =
            null;


        if (reportType == "Person")
        {
            if (!dto.PersonId.HasValue)
            {
                throw new BadRequestException(
                    "A person must be selected.");
            }


            personType =
                NormalizePersonType(
                    dto.PersonType);
        }


        var purchase =
            new ReportPurchase
            {
                UserId =
                    userId,

                ReportType =
                    reportType,

                PersonType =
                    personType,

                PersonId =
                    reportType == "Person"
                        ? dto.PersonId
                        : null,

                AmountInCents =
                    ReportPrice,

                Currency =
                    Currency,

                Status =
                    "Pending",

                CreatedAt =
                    DateTime.UtcNow
            };


        await purchaseRepository
            .AddAsync(purchase);


        await unitOfWork
            .SaveChangesAsync();


        var productName =
            reportType == "Personal"
                ? "My MovieVerse Report"
                : $"{personType} MovieVerse Report";


        var checkout =
            await paymentGateway
                .CreateCheckoutAsync(
                    purchase.Id,
                    userId,
                    productName,
                    purchase.AmountInCents,
                    purchase.Currency);


        purchase.StripeCheckoutSessionId =
            checkout.SessionId;


        purchaseRepository.Update(
            purchase);


        await unitOfWork
            .SaveChangesAsync();


        return new ReportCheckoutReturnDto
        {
            PurchaseId =
                purchase.Id,

            CheckoutUrl =
                checkout.CheckoutUrl
        };
    }


    public async Task<ConfirmedReportPurchaseDto>
        ConfirmPaymentAsync(
            Guid userId,
            string sessionId)
    {
        if (
            string.IsNullOrWhiteSpace(
                sessionId))
        {
            throw new BadRequestException(
                "Stripe session ID is required.");
        }


        var verification =
            await paymentGateway
                .VerifyCheckoutAsync(
                    sessionId);


        if (verification.UserId != userId)
        {
            throw new UnauthorizedException(
                "This payment does not belong to the current user.");
        }


        var purchase =
            await purchaseRepository
                .FirstOrDefaultAsync(
                    x =>
                        x.Id ==
                            verification
                                .PurchaseId
                        &&
                        x.UserId ==
                            userId,
                    tracking: true);


        if (purchase is null)
        {
            throw new NotFoundException(
                "Report purchase was not found.");
        }


        if (
            purchase
                .StripeCheckoutSessionId
            != verification.SessionId)
        {
            throw new BadRequestException(
                "The Stripe session does not match this purchase.");
        }


        if (!verification.IsPaid)
        {
            throw new BadRequestException(
                "Payment has not been completed.");
        }


        if (purchase.Status != "Paid")
        {
            purchase.Status =
                "Paid";

            purchase.PaidAt =
                DateTime.UtcNow;

            purchase.StripePaymentIntentId =
                verification
                    .PaymentIntentId;


            purchaseRepository.Update(
                purchase);


            await unitOfWork
                .SaveChangesAsync();
        }


        return MapPurchase(
            purchase);
    }


    public async Task<ConfirmedReportPurchaseDto>
        GetPaidPurchaseAsync(
            Guid userId,
            Guid purchaseId)
    {
        var purchase =
            await purchaseRepository
                .FirstOrDefaultAsync(
                    x =>
                        x.Id ==
                            purchaseId
                        &&
                        x.UserId ==
                            userId);


        if (purchase is null)
        {
            throw new NotFoundException(
                "Report purchase was not found.");
        }


        if (purchase.Status != "Paid")
        {
            throw new BadRequestException(
                "This report has not been paid for.");
        }


        return MapPurchase(
            purchase);
    }


    private static
        ConfirmedReportPurchaseDto
        MapPurchase(
            ReportPurchase purchase)
    {
        return new ConfirmedReportPurchaseDto
        {
            PurchaseId =
                purchase.Id,

            ReportType =
                purchase.ReportType,

            PersonType =
                purchase.PersonType,

            PersonId =
                purchase.PersonId
        };
    }


    private static string
        NormalizeReportType(
            string? value)
    {
        var normalized =
            value?
                .Trim()
                .ToLowerInvariant();


        return normalized switch
        {
            "personal" =>
                "Personal",

            "person" =>
                "Person",

            _ =>
                throw new BadRequestException(
                    "Report type must be Personal or Person.")
        };
    }


    private static string
        NormalizePersonType(
            string? value)
    {
        var normalized =
            value?
                .Trim()
                .ToLowerInvariant();


        return normalized switch
        {
            "actor" =>
                "Actor",

            "director" =>
                "Director",

            "writer" =>
                "Writer",

            _ =>
                throw new BadRequestException(
                    "Person type must be Actor, Director, or Writer.")
        };
    }
}