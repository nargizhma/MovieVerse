using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieVerse.Dtos.Genres;
using MovieVerse.Exceptions;
using MovieVerse.Services.Interfaces;

namespace MovieVerse.Controllers;

[Route("api/[controller]")]
[ApiController]
public class GenresController(
    IGenreService genreService,
    IValidator<GenreCreateDto> createDtoValidator,
    IValidator<GenreUpdateDto> updateDtoValidator)
    : ControllerBase
{
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll()
    {
        var genres = await genreService.GetAllAsync();

        return Ok(genres);
    }

    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetById(Guid id)
    {
        var genre = await genreService.GetByIdAsync(id);

        return Ok(genre);
    }

    [HttpPost]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public async Task<IActionResult> Create(
        [FromBody] GenreCreateDto dto)
    {
        var validationResult =
            await createDtoValidator.ValidateAsync(dto);

        if (!validationResult.IsValid)
        {
            var errors = string.Join(
                " ",
                validationResult.Errors
                    .Select(x => x.ErrorMessage));

            throw new BadRequestException(errors);
        }

        await genreService.CreateAsync(dto);

        return StatusCode(
            StatusCodes.Status201Created);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] GenreUpdateDto dto)
    {
        var validationResult =
            await updateDtoValidator.ValidateAsync(dto);

        if (!validationResult.IsValid)
        {
            var errors = string.Join(
                " ",
                validationResult.Errors
                    .Select(x => x.ErrorMessage));

            throw new BadRequestException(errors);
        }

        await genreService.UpdateAsync(id, dto);

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await genreService.DeleteAsync(id);

        return NoContent();
    }
}