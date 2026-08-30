using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieVerse.Services.Interfaces;

namespace MovieVerse.Controllers;

[Route("api/admin/reviews")]
[ApiController]
[Authorize(Roles = "Admin,SuperAdmin")]
public class AdminReviewsController(
    IAdminReviewService adminReviewService)
    : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] string? type)
    {
        var reviews =
            await adminReviewService
                .GetAllAsync(type);

        return Ok(reviews);
    }

    [HttpDelete("{type}/{reviewId:guid}")]
    public async Task<IActionResult> Delete(
        string type,
        Guid reviewId)
    {
        await adminReviewService
            .DeleteAsync(
                type,
                reviewId);

        return NoContent();
    }
}
