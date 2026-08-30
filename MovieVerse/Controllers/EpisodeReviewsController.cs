using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieVerse.Dtos.Reviews;
using MovieVerse.Exceptions;
using MovieVerse.Extensions;
using MovieVerse.Services.Interfaces;

namespace MovieVerse.Controllers;

[Route("api/episodes/{episodeId:guid}/reviews")]
[ApiController]
public class EpisodeReviewsController(
    IEpisodeReviewService reviewService,
    IValidator<ReviewCreateDto> createDtoValidator,
    IValidator<ReviewUpdateDto> updateDtoValidator)
    : ControllerBase
{
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll(
        Guid episodeId)
    {
        var reviews =
            await reviewService.GetAllAsync(
                episodeId);

        return Ok(reviews);
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<IActionResult> GetMine(
        Guid episodeId)
    {
        var userId = User.GetUserId();

        var review =
            await reviewService.GetMineAsync(
                episodeId,
                userId);

        return Ok(review);
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Create(
        Guid episodeId,
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
            episodeId,
            userId,
            dto);

        return StatusCode(
            StatusCodes.Status201Created);
    }

    [HttpPut("me")]
    [Authorize]
    public async Task<IActionResult> UpdateMine(
        Guid episodeId,
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
            episodeId,
            userId,
            dto);

        return NoContent();
    }

    [HttpDelete("me")]
    [Authorize]
    public async Task<IActionResult> DeleteMine(
        Guid episodeId)
    {
        var userId = User.GetUserId();

        await reviewService.DeleteMineAsync(
            episodeId,
            userId);

        return NoContent();
    }

    [HttpDelete("{reviewId:guid}")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public async Task<IActionResult> DeleteByAdmin(
        Guid episodeId,
        Guid reviewId)
    {
        await reviewService.DeleteByAdminAsync(
            episodeId,
            reviewId);

        return NoContent();
    }
}
