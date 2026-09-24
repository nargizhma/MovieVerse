using MovieVerse.Dtos.Payments;

namespace MovieVerse.Services.Interfaces;

public interface IReportPaymentService
{
    Task<ReportCheckoutReturnDto>
        CreateCheckoutAsync(
            Guid userId,
            CreateReportCheckoutDto dto);


    Task<ConfirmedReportPurchaseDto>
        ConfirmPaymentAsync(
            Guid userId,
            string sessionId);


    Task<ConfirmedReportPurchaseDto>
        GetPaidPurchaseAsync(
            Guid userId,
            Guid purchaseId);
}