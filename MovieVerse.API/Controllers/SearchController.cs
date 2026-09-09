using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieVerse.Dtos.Search;
using MovieVerse.Services.Interfaces;

namespace MovieVerse.Controllers;

[Route("api/search")]
[ApiController]
public class SearchController(
    IGlobalSearchService searchService)
    : ControllerBase
{
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> Search(
        [FromQuery] string? query,
        [FromQuery] int limit = 5)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            return Ok(
                new GlobalSearchResponseDto());
        }

        var result =
            await searchService.SearchAsync(
                query,
                limit);

        return Ok(result);
    }
}