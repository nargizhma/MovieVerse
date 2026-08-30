using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieVerse.Dtos.Seasons;
using MovieVerse.Exceptions;
using MovieVerse.Services.Interfaces;

namespace MovieVerse.Controllers;

[Route("api")]
[ApiController]
public class SeasonsController(
    ISeasonService seasonService,
    IValidator<SeasonCreateDto> createDtoValidator,
    IValidator<SeasonUpdateDto> updateDtoValidator)
    : ControllerBase
{
    [HttpGet(
        "tvshows/{tvShowId:guid}/seasons")]
    [AllowAnonymous]
    public async Task<IActionResult>
        GetByTVShowId(
            Guid tvShowId)
    {
        var seasons =
            await seasonService
                .GetByTVShowIdAsync(
                    tvShowId);

        return Ok(seasons);
    }

    [HttpGet(
        "seasons/{id:guid}")]
    [AllowAnonymous]
    public async Task<IActionResult>
        GetById(
            Guid id)
    {
        var season =
            await seasonService
                .GetByIdAsync(id);

        return Ok(season);
    }

    [HttpPost(
        "tvshows/{tvShowId:guid}/seasons")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public async Task<IActionResult> Create(
        Guid tvShowId,
        SeasonCreateDto dto)
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

        await seasonService.CreateAsync(
            tvShowId,
            dto);

        return StatusCode(
            StatusCodes.Status201Created);
    }

    [HttpPut(
        "seasons/{id:guid}")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public async Task<IActionResult> Update(
        Guid id,
        SeasonUpdateDto dto)
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

        await seasonService.UpdateAsync(
            id,
            dto);

        return NoContent();
    }

    [HttpDelete(
        "seasons/{id:guid}")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public async Task<IActionResult> Delete(
        Guid id)
    {
        await seasonService.DeleteAsync(
            id);

        return NoContent();
    }
}