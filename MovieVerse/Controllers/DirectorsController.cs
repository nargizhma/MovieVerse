using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieVerse.Dtos.Directors;
using MovieVerse.Exceptions;
using MovieVerse.Services.Interfaces;

namespace MovieVerse.Controllers;

[Route("api/[controller]")]
[ApiController]
public class DirectorsController(
    IDirectorService directorService,
    IValidator<DirectorCreateDto> createDtoValidator,
    IValidator<DirectorUpdateDto> updateDtoValidator)
    : ControllerBase
{
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll()
    {
        var directors =
            await directorService.GetAllAsync();

        return Ok(directors);
    }

    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetById(Guid id)
    {
        var director =
            await directorService.GetByIdAsync(id);

        return Ok(director);
    }

    [HttpPost]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public async Task<IActionResult> Create(
        [FromForm] DirectorCreateDto dto)
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

        await directorService.CreateAsync(dto);

        return StatusCode(
            StatusCodes.Status201Created);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public async Task<IActionResult> Update(
        Guid id,
        [FromForm] DirectorUpdateDto dto)
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

        await directorService.UpdateAsync(id, dto);

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await directorService.DeleteAsync(id);

        return NoContent();
    }
}