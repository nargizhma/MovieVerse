using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using MovieVerse.Dtos.Auth;
using MovieVerse.Exceptions;
using MovieVerse.Services.Interfaces;

namespace MovieVerse.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController(
    IValidator<RegisterDto> registerDtoValidator,
    IValidator<LoginDto> loginDtoValidator,
    IAuthService authService)
    : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> Register(
        RegisterDto dto)
    {
        var validationResult =
            await registerDtoValidator.ValidateAsync(dto);

        if (!validationResult.IsValid)
        {
            var errors = string.Join(
                " ",
                validationResult.Errors
                    .Select(x => x.ErrorMessage));

            throw new BadRequestException(errors);
        }

        var result =
            await authService.RegisterAsync(dto);

        return Ok(result);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(
        LoginDto dto)
    {
        var validationResult =
            await loginDtoValidator.ValidateAsync(dto);

        if (!validationResult.IsValid)
        {
            var errors = string.Join(
                " ",
                validationResult.Errors
                    .Select(x => x.ErrorMessage));

            throw new BadRequestException(errors);
        }

        var result =
            await authService.LoginAsync(dto);

        return Ok(result);
    }
}