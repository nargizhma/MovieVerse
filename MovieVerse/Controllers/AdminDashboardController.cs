using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieVerse.Services.Interfaces;

namespace MovieVerse.Controllers;

[Route("api/admin/dashboard")]
[ApiController]
[Authorize(Roles = "Admin,SuperAdmin")]
public class AdminDashboardController(
    IAdminDashboardService dashboardService)
    : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetStats()
    {
        var stats =
            await dashboardService
                .GetStatsAsync();

        return Ok(stats);
    }
}
