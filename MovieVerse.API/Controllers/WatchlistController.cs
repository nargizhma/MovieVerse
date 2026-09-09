using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieVerse.Extensions;
using MovieVerse.Services.Interfaces;

namespace MovieVerse.Controllers;

[Route("api/watchlist")]
[ApiController]
[Authorize]
public class WatchlistController(
    IWatchlistService watchlistService)
    : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetMine()
    {
        var userId = User.GetUserId();

        var items =
            await watchlistService.GetMineAsync(
                userId);

        return Ok(items);
    }

    [HttpPost("movies/{movieId:guid}")]
    public async Task<IActionResult> AddMovie(
        Guid movieId)
    {
        var userId = User.GetUserId();

        await watchlistService.AddMovieAsync(
            userId,
            movieId);

        return StatusCode(
            StatusCodes.Status201Created);
    }

    [HttpDelete("movies/{movieId:guid}")]
    public async Task<IActionResult> RemoveMovie(
        Guid movieId)
    {
        var userId = User.GetUserId();

        await watchlistService.RemoveMovieAsync(
            userId,
            movieId);

        return NoContent();
    }

    [HttpPost("tvshows/{tvShowId:guid}")]
    public async Task<IActionResult> AddTVShow(
        Guid tvShowId)
    {
        var userId = User.GetUserId();

        await watchlistService.AddTVShowAsync(
            userId,
            tvShowId);

        return StatusCode(
            StatusCodes.Status201Created);
    }

    [HttpDelete("tvshows/{tvShowId:guid}")]
    public async Task<IActionResult> RemoveTVShow(
        Guid tvShowId)
    {
        var userId = User.GetUserId();

        await watchlistService.RemoveTVShowAsync(
            userId,
            tvShowId);

        return NoContent();
    }
}
