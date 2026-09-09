using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieVerse.Dtos.Reviews;
using MovieVerse.Exceptions;
using MovieVerse.Extensions;
using MovieVerse.Services.Interfaces;

namespace MovieVerse.Controllers;

[Route("api/movies/{movieId:guid}/reviews")]
[ApiController]
public class MovieReviewsController(
    IMovieReviewService reviewService,
    IValidator<ReviewCreateDto> createDtoValidator,
    IValidator<ReviewUpdateDto> updateDtoValidator)
    : ControllerBase
{
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll(
        Guid movieId)
    {
        var reviews =
            await reviewService.GetAllAsync(
                movieId);

        return Ok(reviews);
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<IActionResult> GetMine(
        Guid movieId)
    {
        var userId = User.GetUserId();

        var review =
            await reviewService.GetMineAsync(
                movieId,
                userId);

        return Ok(review);
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Create(
        Guid movieId,
        ReviewCreateDto dto)
    {
        var validationResult =
            await createDtoValidator
                .ValidateAsync(dto);

        if (!validationResult.IsValid)
        {
            var errors = string.Join(
                " ",
                validationResult.Errors
                    .Select(x => x.ErrorMessage));

            throw new BadRequestException(errors);
        }

        var userId = User.GetUserId();

        await reviewService.CreateAsync(
            movieId,
            userId,
            dto);

        return StatusCode(
            StatusCodes.Status201Created);
    }

    [HttpPut("me")]
    [Authorize]
    public async Task<IActionResult> UpdateMine(
        Guid movieId,
        ReviewUpdateDto dto)
    {
        var validationResult =
            await updateDtoValidator
                .ValidateAsync(dto);

        if (!validationResult.IsValid)
        {
            var errors = string.Join(
                " ",
                validationResult.Errors
                    .Select(x => x.ErrorMessage));

            throw new BadRequestException(errors);
        }

        var userId = User.GetUserId();

        await reviewService.UpdateAsync(
            movieId,
            userId,
            dto);

        return NoContent();
    }

    [HttpDelete("me")]
    [Authorize]
    public async Task<IActionResult> DeleteMine(
        Guid movieId)
    {
        var userId = User.GetUserId();

        await reviewService.DeleteMineAsync(
            movieId,
            userId);

        return NoContent();
    }

    [HttpDelete("{reviewId:guid}")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public async Task<IActionResult> DeleteByAdmin(
        Guid movieId,
        Guid reviewId)
    {
        await reviewService.DeleteByAdminAsync(
            movieId,
            reviewId);

        return NoContent();
    }
}
