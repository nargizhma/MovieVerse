using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MovieVerse.Data;
using MovieVerse.Dtos.Auth;
using MovieVerse.Models;
using MovieVerse.Services.Interfaces;

namespace MovieVerse.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController(
    UserManager<AppUser> userManager,
    AppDbContext dbContext,
    IMapper mapper,
    IJwtService jwtService) : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto dto)
    {
        var existingUser = await userManager.FindByEmailAsync(dto.Email);

        if (existingUser is not null)
            return BadRequest("A user with this email already exists.");

        var existingUserName = await userManager.FindByNameAsync(dto.UserName);

        if (existingUserName is not null)
            return BadRequest("This username is already taken.");

        var user = mapper.Map<AppUser>(dto);

        var result = await userManager.CreateAsync(user, dto.Password);

        if (!result.Succeeded)
        {
            return BadRequest(result.Errors.Select(x => x.Description));
        }

        var roleResult = await userManager.AddToRoleAsync(user, "User");

        if (!roleResult.Succeeded)
        {
            await userManager.DeleteAsync(user);

            return BadRequest(
                roleResult.Errors.Select(x => x.Description));
        }

        try
        {
            var profile = mapper.Map<UserProfile>(dto);
            profile.AppUserId = user.Id;

            dbContext.UserProfiles.Add(profile);
            await dbContext.SaveChangesAsync();
        }
        catch
        {
            await userManager.DeleteAsync(user);
            throw;
        }

        var token = await jwtService.CreateTokenAsync(user);

        return Ok(new AuthResponseDto
        {
            Token = token
        });
    }


    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto dto)
    {
        var user = await userManager.FindByEmailAsync(dto.Email);

        if (user is null)
            return Unauthorized("Invalid email or password.");

        var isPasswordValid =
            await userManager.CheckPasswordAsync(user, dto.Password);

        if (!isPasswordValid)
            return Unauthorized("Invalid email or password.");

        var token = await jwtService.CreateTokenAsync(user);

        return Ok(new AuthResponseDto
        {
            Token = token
        });
    }
}