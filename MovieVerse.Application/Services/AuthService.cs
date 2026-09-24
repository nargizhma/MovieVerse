using MovieVerse.Abstractions.Email;
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
    IJwtService jwtService,
    IEmailService emailService)
    : IAuthService
{
    public async Task<RegisterResponseDto>
        RegisterAsync(RegisterDto dto)
    {
        var existingEmail =
            await identityService
                .FindByEmailAsync(
                    dto.Email);

        if (existingEmail is not null)
        {
            throw new AlreadyExistsException(
                "A user with this email already exists.");
        }

        var existingUserName =
            await identityService
                .FindByUserNameAsync(
                    dto.UserName);

        if (existingUserName is not null)
        {
            throw new AlreadyExistsException(
                "This username is already taken.");
        }

        await using var transaction =
            await unitOfWork
                .BeginTransactionAsync();

        try
        {
            var createResult =
                await identityService
                    .CreateAsync(
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

            var confirmationToken =
                await identityService
                    .GenerateEmailConfirmationTokenAsync(
                        userId);

            if (string.IsNullOrWhiteSpace(
                    confirmationToken))
            {
                throw new BadRequestException(
                    "Could not create email verification token.");
            }

            await emailService
                .SendEmailConfirmationAsync(
                    dto.Email,
                    userId,
                    confirmationToken);

            await transaction.CommitAsync();

            return new RegisterResponseDto
            {
                UserId = userId,
                Email = dto.Email,
                RequiresEmailConfirmation =
                    true
            };
        }
        catch
        {
            await transaction
                .RollbackAsync();

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
        {
            throw new UnauthorizedException(
                "Invalid email or password.");
        }

        var valid =
            await identityService
                .CheckPasswordAsync(
                    user.Id,
                    dto.Password);

        if (!valid)
        {
            throw new UnauthorizedException(
                "Invalid email or password.");
        }

        if (!user.EmailConfirmed)
        {
            throw new UnauthorizedException(
                "Please verify your email before logging in.");
        }

        var token =
            await jwtService
                .CreateTokenAsync(
                    user.Id);

        return new AuthResponseDto
        {
            Token = token
        };
    }

    public async Task ConfirmEmailAsync(
        Guid userId,
        string token)
    {
        var user =
            await identityService
                .FindByIdAsync(userId);

        if (user is null)
        {
            throw new NotFoundException(
                "User was not found.");
        }

        if (user.EmailConfirmed)
            return;

        var result =
            await identityService
                .ConfirmEmailAsync(
                    userId,
                    token);

        if (!result.Succeeded)
        {
            throw new BadRequestException(
                "The verification link is invalid or has expired.");
        }
    }

    public async Task
        ResendConfirmationEmailAsync(
            string email)
    {
        var user =
            await identityService
                .FindByEmailAsync(email);

        // Intentionally do nothing so this
        // endpoint doesn't reveal registered emails.
        if (user is null ||
            user.EmailConfirmed)
        {
            return;
        }

        var confirmationToken =
            await identityService
                .GenerateEmailConfirmationTokenAsync(
                    user.Id);

        if (string.IsNullOrWhiteSpace(
                confirmationToken))
        {
            return;
        }

        await emailService
            .SendEmailConfirmationAsync(
                user.Email,
                user.Id,
                confirmationToken);
    }
}