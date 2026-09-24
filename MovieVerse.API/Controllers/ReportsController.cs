using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieVerse.Extensions;
using MovieVerse.Services.Interfaces;

namespace MovieVerse.Controllers;

[Route("api/reports")]
[ApiController]
[Authorize]
public class ReportsController(
    IReportService reportService,
    IReportPaymentService paymentService)
    : ControllerBase
{
    [HttpGet("purchase/{purchaseId:guid}")]
    [Produces("application/pdf")]
    [ProducesResponseType(
        StatusCodes.Status200OK)]
    public async Task<IActionResult>
        GeneratePurchasedReport(
            Guid purchaseId)
    {
        var userId =
            User.GetUserId();


        var purchase =
            await paymentService
                .GetPaidPurchaseAsync(
                    userId,
                    purchaseId);


        if (
            purchase.ReportType ==
            "Personal")
        {
            var report =
                await reportService
                    .GenerateMyReportAsync(
                        userId);


            return File(
                report.Content,
                "application/pdf",
                report.FileName);
        }


        if (
            purchase.ReportType ==
                "Person"
            &&
            purchase.PersonId
                .HasValue
            &&
            !string.IsNullOrWhiteSpace(
                purchase.PersonType))
        {
            var report =
                await reportService
                    .GeneratePersonReportAsync(
                        purchase.PersonType,
                        purchase.PersonId.Value);


            return File(
                report.Content,
                "application/pdf",
                report.FileName);
        }


        throw new InvalidOperationException(
            "The purchased report configuration is invalid.");
    }
}