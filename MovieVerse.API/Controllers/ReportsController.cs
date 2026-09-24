using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using MovieVerse.Extensions;
using MovieVerse.Services.Interfaces;

namespace MovieVerse.Controllers;

[Route("api/reports")]
[ApiController]
[Authorize]
public class ReportsController(
    IReportService reportService)
    : ControllerBase
{
    [HttpGet("me")]
    [Produces("application/pdf")]
    [ProducesResponseType(
        StatusCodes.Status200OK)]
    public async Task<IActionResult>
        GenerateMyReport()
    {
        var userId =
            User.GetUserId();


        var report =
            await reportService
                .GenerateMyReportAsync(
                    userId);


        return File(
            report.Content,
            "application/pdf",
            report.FileName);
    }


    [HttpGet(
        "person/{personType}/{personId:guid}")]
    [Produces("application/pdf")]
    [ProducesResponseType(
        StatusCodes.Status200OK)]
    public async Task<IActionResult>
        GeneratePersonReport(
            string personType,
            Guid personId)
    {
        var report =
            await reportService
                .GeneratePersonReportAsync(
                    personType,
                    personId);


        return File(
            report.Content,
            "application/pdf",
            report.FileName);
    }
}