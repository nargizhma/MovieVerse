using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieVerse.Dtos.Reviews;
using MovieVerse.Exceptions;
using MovieVerse.Extensions;
using MovieVerse.Services.Interfaces;

namespace MovieVerse.Controllers;

[Route("api/tvshows/{tvShowId:guid}/reviews")]
[ApiController]
public class TVShowReviewsController(
    ITVShowReviewService reviewService,
    IValidator<ReviewCreateDto> createDtoValidator,
    IValidator<ReviewUpdateDto> updateDtoValidator)
    : ControllerBase
{
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll(
        Guid tvShowId)
    {
        var reviews =
            await reviewService.GetAllAsync(
                tvShowId);

        return Ok(reviews);
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<IActionResult> GetMine(
        Guid tvShowId)
    {
        var userId = User.GetUserId();

        var review =
            await reviewService.GetMineAsync(
                tvShowId,
                userId);

        return Ok(review);
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Create(
        Guid tvShowId,
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
            tvShowId,
            userId,
            dto);

        return StatusCode(
            StatusCodes.Status201Created);
    }

    [HttpPut("me")]
    [Authorize]
    public async Task<IActionResult> UpdateMine(
        Guid tvShowId,
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
            tvShowId,
            userId,
            dto);

        return NoContent();
    }

    [HttpDelete("me")]
    [Authorize]
    public async Task<IActionResult> DeleteMine(
        Guid tvShowId)
    {
        var userId = User.GetUserId();

        await reviewService.DeleteMineAsync(
            tvShowId,
            userId);

        return NoContent();
    }

    [HttpDelete("{reviewId:guid}")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public async Task<IActionResult> DeleteByAdmin(
        Guid tvShowId,
        Guid reviewId)
    {
        await reviewService.DeleteByAdminAsync(
            tvShowId,
            reviewId);

        return NoContent();
    }
}
