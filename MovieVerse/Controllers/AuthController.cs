using Microsoft.AspNetCore.Mvc;
using MovieVerse.Dtos.Auth;
using MovieVerse.Services.Interfaces;

namespace MovieVerse.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController(
    IAuthService authService)
    : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> Register(
        RegisterDto dto)
    {
        var result =
            await authService.RegisterAsync(dto);

        return Ok(result);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(
        LoginDto dto)
    {
        var result =
            await authService.LoginAsync(dto);

        return Ok(result);
    }
}