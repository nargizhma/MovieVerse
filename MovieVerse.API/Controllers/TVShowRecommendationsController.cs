using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieVerse.Dtos.Recommendations;
using MovieVerse.Exceptions;
using MovieVerse.Services.Interfaces;

namespace MovieVerse.Controllers;

[Route("api/tvshows")]
[ApiController]
public class TVShowRecommendationsController(
    ITVShowRecommendationService recommendationService,
    IValidator<SimilarTVShowsQueryDto> queryValidator)
    : ControllerBase
{
    [HttpGet("{tvShowId:guid}/similar")]
    [AllowAnonymous]
    public async Task<IActionResult> GetSimilarTVShows(
        Guid tvShowId,
        [FromQuery] SimilarTVShowsQueryDto query)
    {
        var validationResult =
            await queryValidator
                .ValidateAsync(query);

        if (!validationResult.IsValid)
        {
            var errors =
                string.Join(
                    " ",
                    validationResult.Errors
                        .Select(x =>
                            x.ErrorMessage));

            throw new BadRequestException(
                errors);
        }

        var tvShows =
            await recommendationService
                .GetSimilarTVShowsAsync(
                    tvShowId,
                    query.Limit);

        return Ok(tvShows);
    }
}