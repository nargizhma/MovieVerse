using AutoMapper;
using Microsoft.AspNetCore.Identity;
using MovieVerse.Data;
using MovieVerse.Dtos.Auth;
using MovieVerse.Exceptions;
using MovieVerse.Models;
using MovieVerse.Services.Interfaces;

namespace MovieVerse.Services;

public class AuthService(
    UserManager<AppUser> userManager,
    AppDbContext dbContext,
    IMapper mapper,
    IJwtService jwtService)
    : IAuthService
{
    public async Task<AuthResponseDto> RegisterAsync(RegisterDto dto)
    {
        var existingEmail =
            await userManager.FindByEmailAsync(dto.Email);

        if (existingEmail is not null)
            throw new AlreadyExistsException(
                "A user with this email already exists.");

        var existingUserName =
            await userManager.FindByNameAsync(dto.UserName);

        if (existingUserName is not null)
            throw new AlreadyExistsException(
                "This username is already taken.");

        await using var transaction =
            await dbContext.Database.BeginTransactionAsync();

        var user = mapper.Map<AppUser>(dto);

        var result =
            await userManager.CreateAsync(user, dto.Password);

        if (!result.Succeeded)
        {
            var errors = string.Join(
                " ",
                result.Errors.Select(x => x.Description));

            throw new BadRequestException(errors);
        }

        var roleResult =
            await userManager.AddToRoleAsync(user, "User");

        if (!roleResult.Succeeded)
        {
            var errors = string.Join(
                " ",
                roleResult.Errors.Select(x => x.Description));

            throw new BadRequestException(errors);
        }

        var profile = mapper.Map<UserProfile>(dto);

        profile.AppUserId = user.Id;

        dbContext.UserProfiles.Add(profile);

        await dbContext.SaveChangesAsync();

        var token = await jwtService.CreateTokenAsync(user);

        await transaction.CommitAsync();

        return new AuthResponseDto
        {
            Token = token
        };
    }

    public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
    {
        var user =
            await userManager.FindByEmailAsync(dto.Email);

        if (user is null)
            throw new UnauthorizedException(
                "Invalid email or password.");

        var isPasswordValid =
            await userManager.CheckPasswordAsync(
                user,
                dto.Password);

        if (!isPasswordValid)
            throw new UnauthorizedException(
                "Invalid email or password.");

        var token =
            await jwtService.CreateTokenAsync(user);

        return new AuthResponseDto
        {
            Token = token
        };
    }
}