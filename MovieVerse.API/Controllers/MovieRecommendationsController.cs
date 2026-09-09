using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieVerse.Dtos.Recommendations;
using MovieVerse.Exceptions;
using MovieVerse.Services.Interfaces;

namespace MovieVerse.Controllers;

[Route("api/movies")]
[ApiController]
public class MovieRecommendationsController(
    IMovieRecommendationService recommendationService,
    IValidator<SimilarMoviesQueryDto> queryValidator)
    : ControllerBase
{
    [HttpGet("{movieId:guid}/similar")]
    [AllowAnonymous]
    public async Task<IActionResult> GetSimilarMovies(
        Guid movieId,
        [FromQuery] SimilarMoviesQueryDto query)
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

        var movies =
            await recommendationService
                .GetSimilarMoviesAsync(
                    movieId,
                    query.Limit);

        return Ok(movies);
    }
}
