using MovieVerse.Abstractions.Identity;
using MovieVerse.Abstractions.Persistence;
using MovieVerse.Dtos.Auth;
using MovieVerse.Exceptions;
using MovieVerse.Models;
using MovieVerse.Repositories.Interfaces;
using MovieVerse.Services.Interfaces;

namespace MovieVerse.Services;

public class AuthService(
    IIdentityService identityService,
    IUserProfileRepository profileRepository,
    IUnitOfWork unitOfWork,
    IJwtService jwtService)
    : IAuthService
{
    public async Task<AuthResponseDto>
        RegisterAsync(RegisterDto dto)
    {
        var existingEmail =
            await identityService
                .FindByEmailAsync(
                    dto.Email);

        if (existingEmail is not null)
            throw new AlreadyExistsException(
                "A user with this email already exists.");

        var existingUserName =
            await identityService
                .FindByUserNameAsync(
                    dto.UserName);

        if (existingUserName is not null)
            throw new AlreadyExistsException(
                "This username is already taken.");

        await using var transaction =
            await unitOfWork
                .BeginTransactionAsync();

        try
        {
            var createResult =
                await identityService.CreateAsync(
                    dto.Email,
                    dto.UserName,
                    dto.Password);

            if (!createResult.Succeeded ||
                !createResult.UserId.HasValue)
            {
                throw new BadRequestException(
                    string.Join(
                        " ",
                        createResult.Errors));
            }

            var userId =
                createResult.UserId.Value;

            var roleResult =
                await identityService
                    .AddToRoleAsync(
                        userId,
                        "User");

            if (!roleResult.Succeeded)
            {
                throw new BadRequestException(
                    string.Join(
                        " ",
                        roleResult.Errors));
            }

            await profileRepository.AddAsync(
                new UserProfile
                {
                    AppUserId =
                        userId,

                    DisplayName =
                        dto.DisplayName
                });

            await unitOfWork
                .SaveChangesAsync();

            var token =
                await jwtService
                    .CreateTokenAsync(
                        userId);

            await transaction.CommitAsync();

            return new AuthResponseDto
            {
                Token = token
            };
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<AuthResponseDto>
        LoginAsync(LoginDto dto)
    {
        var user =
            await identityService
                .FindByEmailAsync(
                    dto.Email);

        if (user is null)
            throw new UnauthorizedException(
                "Invalid email or password.");

        var valid =
            await identityService
                .CheckPasswordAsync(
                    user.Id,
                    dto.Password);

        if (!valid)
            throw new UnauthorizedException(
                "Invalid email or password.");

        var token =
            await jwtService
                .CreateTokenAsync(
                    user.Id);

        return new AuthResponseDto
        {
            Token = token
        };
    }
}