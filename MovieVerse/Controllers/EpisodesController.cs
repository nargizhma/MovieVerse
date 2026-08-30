using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieVerse.Dtos.Episodes;
using MovieVerse.Exceptions;
using MovieVerse.Services.Interfaces;

namespace MovieVerse.Controllers;

[Route("api")]
[ApiController]
public class EpisodesController(
    IEpisodeService episodeService,
    IValidator<EpisodeCreateDto> createDtoValidator,
    IValidator<EpisodeUpdateDto> updateDtoValidator)
    : ControllerBase
{
    [HttpGet(
        "seasons/{seasonId:guid}/episodes")]
    [AllowAnonymous]
    public async Task<IActionResult>
        GetBySeasonId(
            Guid seasonId)
    {
        var episodes =
            await episodeService
                .GetBySeasonIdAsync(
                    seasonId);

        return Ok(episodes);
    }

    [HttpGet(
        "episodes/{id:guid}")]
    [AllowAnonymous]
    public async Task<IActionResult>
        GetById(
            Guid id)
    {
        var episode =
            await episodeService
                .GetByIdAsync(id);

        return Ok(episode);
    }

    [HttpPost(
        "seasons/{seasonId:guid}/episodes")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public async Task<IActionResult> Create(
        Guid seasonId,
        [FromForm] EpisodeCreateDto dto)
    {
        var validationResult =
            await createDtoValidator
                .ValidateAsync(dto);

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

        await episodeService.CreateAsync(
            seasonId,
            dto);

        return StatusCode(
            StatusCodes.Status201Created);
    }

    [HttpPut(
        "episodes/{id:guid}")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public async Task<IActionResult> Update(
        Guid id,
        [FromForm] EpisodeUpdateDto dto)
    {
        var validationResult =
            await updateDtoValidator
                .ValidateAsync(dto);

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

        await episodeService.UpdateAsync(
            id,
            dto);

        return NoContent();
    }

    [HttpDelete(
        "episodes/{id:guid}")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public async Task<IActionResult> Delete(
        Guid id)
    {
        await episodeService.DeleteAsync(
            id);

        return NoContent();
    }
}