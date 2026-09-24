using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using MovieVerse.Dtos.Auth;
using MovieVerse.Exceptions;
using MovieVerse.Hubs;
using MovieVerse.Services.Interfaces;

namespace MovieVerse.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController(
    IValidator<RegisterDto>
        registerDtoValidator,
    IValidator<LoginDto>
        loginDtoValidator,
    IValidator<ConfirmEmailDto>
        confirmEmailDtoValidator,
    IValidator<ResendConfirmationEmailDto>
        resendValidator,
    IAuthService authService,
    IHubContext<EmailVerificationHub>
        verificationHub)
    : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult>
        Register(RegisterDto dto)
    {
        var validationResult =
            await registerDtoValidator
                .ValidateAsync(dto);

        if (!validationResult.IsValid)
        {
            throw new BadRequestException(
                string.Join(
                    " ",
                    validationResult.Errors
                        .Select(x =>
                            x.ErrorMessage)));
        }

        var result =
            await authService
                .RegisterAsync(dto);

        return Ok(result);
    }

    [HttpPost("login")]
    public async Task<IActionResult>
        Login(LoginDto dto)
    {
        var validationResult =
            await loginDtoValidator
                .ValidateAsync(dto);

        if (!validationResult.IsValid)
        {
            throw new BadRequestException(
                string.Join(
                    " ",
                    validationResult.Errors
                        .Select(x =>
                            x.ErrorMessage)));
        }

        var result =
            await authService
                .LoginAsync(dto);

        return Ok(result);
    }

    [HttpPost("confirm-email")]
    public async Task<IActionResult>
        ConfirmEmail(
            ConfirmEmailDto dto)
    {
        var validationResult =
            await confirmEmailDtoValidator
                .ValidateAsync(dto);

        if (!validationResult.IsValid)
        {
            throw new BadRequestException(
                string.Join(
                    " ",
                    validationResult.Errors
                        .Select(x =>
                            x.ErrorMessage)));
        }

        await authService
            .ConfirmEmailAsync(
                dto.UserId,
                dto.Token);

        await verificationHub
            .Clients
            .Group(
                EmailVerificationHub
                    .GetGroupName(
                        dto.UserId))
            .SendAsync(
                "EmailVerified");

        return Ok(
            new
            {
                message =
                    "Email verified successfully."
            });
    }

    [HttpPost("resend-confirmation")]
    public async Task<IActionResult>
        ResendConfirmation(
            ResendConfirmationEmailDto dto)
    {
        var validationResult =
            await resendValidator
                .ValidateAsync(dto);

        if (!validationResult.IsValid)
        {
            throw new BadRequestException(
                string.Join(
                    " ",
                    validationResult.Errors
                        .Select(x =>
                            x.ErrorMessage)));
        }

        await authService
            .ResendConfirmationEmailAsync(
                dto.Email);

        return Ok(
            new
            {
                message =
                    "If this email belongs to an unverified account, a verification email has been sent."
            });
    }
}