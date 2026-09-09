using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieVerse.Dtos.Common;
using MovieVerse.Dtos.Movies;
using MovieVerse.Exceptions;
using MovieVerse.Requests.Movies;
using MovieVerse.Services.Interfaces;

namespace MovieVerse.Controllers;

[Route("api/[controller]")]
[ApiController]
public class MoviesController(
    IMovieService movieService,
    IValidator<MovieCreateDto> createDtoValidator,
    IValidator<MovieUpdateDto> updateDtoValidator,
    IValidator<CatalogFilterDto> filterValidator,
    IMapper mapper)
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

        var movies =
            await movieService.GetAllAsync(
                filter);

        return Ok(movies);
    }

    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetById(
        Guid id)
    {
        var movie =
            await movieService.GetByIdAsync(id);

        return Ok(movie);
    }

    [HttpPost]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public async Task<IActionResult> Create(
        [FromForm] MovieCreateDto dto)
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

        var request =
            mapper.Map<MovieCreateRequest>(dto);

        await movieService.CreateAsync(request);

        return StatusCode(
            StatusCodes.Status201Created);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public async Task<IActionResult> Update(
        Guid id,
        [FromForm] MovieUpdateDto dto)
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

        var request =
            mapper.Map<MovieUpdateRequest>(dto);

        await movieService.UpdateAsync(
            id,
            request);

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public async Task<IActionResult> Delete(
        Guid id)
    {
        await movieService.DeleteAsync(id);

        return NoContent();
    }
}
