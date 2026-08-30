using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieVerse.Dtos.Profiles;
using MovieVerse.Exceptions;
using MovieVerse.Extensions;
using MovieVerse.Services.Interfaces;

namespace MovieVerse.Controllers;

[Route("api/profiles")]
[ApiController]
public class ProfilesController(
    IUserProfileService profileService,
    IValidator<ProfileUpdateDto> updateValidator)
    : ControllerBase
{
    [HttpGet("me")]
    [Authorize]
    public async Task<IActionResult> GetMine()
    {
        var userId =
            User.GetUserId();

        var profile =
            await profileService
                .GetMineAsync(userId);

        return Ok(profile);
    }

    [HttpGet("me/activity")]
    [Authorize]
    public async Task<IActionResult> GetMyActivity()
    {
        var userId =
            User.GetUserId();

        var activity =
            await profileService
                .GetMyActivityAsync(
                    userId);

        return Ok(activity);
    }

    [HttpGet("{userName}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetByUserName(
        string userName)
    {
        var profile =
            await profileService
                .GetByUserNameAsync(
                    userName);

        return Ok(profile);
    }

    [HttpPut("me")]
    [Authorize]
    public async Task<IActionResult> UpdateMine(
        [FromForm] ProfileUpdateDto dto)
    {
        var validationResult =
            await updateValidator
                .ValidateAsync(dto);

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

        var userId =
            User.GetUserId();

        await profileService.UpdateAsync(
            userId,
            dto);

        return NoContent();
    }

    [HttpDelete("me/image")]
    [Authorize]
    public async Task<IActionResult>
        DeleteMyProfileImage()
    {
        var userId =
            User.GetUserId();

        await profileService
            .DeleteProfileImageAsync(
                userId);

        return NoContent();
    }
}
