using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieVerse.Dtos.Payments;
using MovieVerse.Extensions;
using MovieVerse.Services.Interfaces;

namespace MovieVerse.Controllers;

[Route("api/report-payments")]
[ApiController]
[Authorize]
public class ReportPaymentsController(
    IReportPaymentService paymentService)
    : ControllerBase
{
    [HttpPost("checkout")]
    public async Task<IActionResult>
        CreateCheckout(
            CreateReportCheckoutDto dto)
    {
        var userId =
            User.GetUserId();


        var result =
            await paymentService
                .CreateCheckoutAsync(
                    userId,
                    dto);


        return Ok(result);
    }


    [HttpPost("confirm")]
    public async Task<IActionResult>
        Confirm(
            [FromQuery]
            string sessionId)
    {
        var userId =
            User.GetUserId();


        var result =
            await paymentService
                .ConfirmPaymentAsync(
                    userId,
                    sessionId);


        return Ok(result);
    }
}