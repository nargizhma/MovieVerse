using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieVerse.Dtos.Common;
using MovieVerse.Dtos.TVShows;
using MovieVerse.Exceptions;
using MovieVerse.Services.Interfaces;

namespace MovieVerse.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TVShowsController(
    ITVShowService tvShowService,
    IValidator<TVShowCreateDto> createDtoValidator,
    IValidator<TVShowUpdateDto> updateDtoValidator,
    IValidator<CatalogFilterDto> filterValidator)
    : ControllerBase
{
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll(
        [FromQuery] CatalogFilterDto filter)
    {
        var validationResult =
            await filterValidator
                .ValidateAsync(filter);

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
            await tvShowService.GetAllAsync(
                filter);

        return Ok(tvShows);
    }

    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetById(
        Guid id)
    {
        var tvShow =
            await tvShowService.GetByIdAsync(
                id);

        return Ok(tvShow);
    }

    [HttpPost]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public async Task<IActionResult> Create(
        [FromForm] TVShowCreateDto dto)
    {
        var validationResult =
            await createDtoValidator
                .ValidateAsync(dto);

        if (!validationResult.IsValid)
        {
            var errors = string.Join(
                " ",
                validationResult.Errors
                    .Select(x =>
                        x.ErrorMessage));

            throw new BadRequestException(
                errors);
        }

        await tvShowService.CreateAsync(
            dto);

        return StatusCode(
            StatusCodes.Status201Created);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public async Task<IActionResult> Update(
        Guid id,
        [FromForm] TVShowUpdateDto dto)
    {
        var validationResult =
            await updateDtoValidator
                .ValidateAsync(dto);

        if (!validationResult.IsValid)
        {
            var errors = string.Join(
                " ",
                validationResult.Errors
                    .Select(x =>
                        x.ErrorMessage));

            throw new BadRequestException(
                errors);
        }

        await tvShowService.UpdateAsync(
            id,
            dto);

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public async Task<IActionResult> Delete(
        Guid id)
    {
        await tvShowService.DeleteAsync(
            id);

        return NoContent();
    }
}
