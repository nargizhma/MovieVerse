using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieVerse.Extensions;
using MovieVerse.Services.Interfaces;

namespace MovieVerse.Controllers;

[Route("api/watch-history")]
[ApiController]
[Authorize]
public class WatchHistoryController(
    IWatchHistoryService watchHistoryService)
    : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetMine()
    {
        var userId = User.GetUserId();

        var items =
            await watchHistoryService.GetMineAsync(
                userId);

        return Ok(items);
    }

    [HttpPost("movies/{movieId:guid}")]
    public async Task<IActionResult> MarkMovieWatched(
        Guid movieId)
    {
        var userId = User.GetUserId();

        await watchHistoryService
            .MarkMovieWatchedAsync(
                userId,
                movieId);

        return StatusCode(
            StatusCodes.Status201Created);
    }

    [HttpDelete("movies/{movieId:guid}")]
    public async Task<IActionResult> MarkMovieUnwatched(
        Guid movieId)
    {
        var userId = User.GetUserId();

        await watchHistoryService
            .MarkMovieUnwatchedAsync(
                userId,
                movieId);

        return NoContent();
    }

    [HttpPost("tvshows/{tvShowId:guid}")]
    public async Task<IActionResult> MarkTVShowWatched(
        Guid tvShowId)
    {
        var userId = User.GetUserId();

        await watchHistoryService
            .MarkTVShowWatchedAsync(
                userId,
                tvShowId);

        return StatusCode(
            StatusCodes.Status201Created);
    }

    [HttpDelete("tvshows/{tvShowId:guid}")]
    public async Task<IActionResult> MarkTVShowUnwatched(
        Guid tvShowId)
    {
        var userId = User.GetUserId();

        await watchHistoryService
            .MarkTVShowUnwatchedAsync(
                userId,
                tvShowId);

        return NoContent();
    }
}
