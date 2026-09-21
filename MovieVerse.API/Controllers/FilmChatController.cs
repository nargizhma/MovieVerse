using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieVerse.Dtos.AI;
using MovieVerse.Exceptions;
using MovieVerse.Services.Interfaces;

namespace MovieVerse.Controllers;

[Route("api/ai/film-chat")]
[ApiController]
[Authorize]
public class FilmChatController(
    IFilmChatService filmChatService,
    IValidator<FilmChatRequestDto> validator)
    : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Send(
        [FromBody] FilmChatRequestDto request,
        CancellationToken cancellationToken)
    {
        var validationResult =
            await validator.ValidateAsync(
                request,
                cancellationToken);

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

        var result =
            await filmChatService.SendAsync(
                request,
                cancellationToken);

        return Ok(result);
    }
}