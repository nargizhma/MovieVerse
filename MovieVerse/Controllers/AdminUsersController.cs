using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieVerse.Dtos.Admin;
using MovieVerse.Exceptions;
using MovieVerse.Extensions;
using MovieVerse.Services.Interfaces;

namespace MovieVerse.Controllers;

[Route("api/admin/users")]
[ApiController]
[Authorize(Roles = "Admin,SuperAdmin")]
public class AdminUsersController(
    IAdminUserService adminUserService,
    IValidator<AdminUserRoleUpdateDto>
        roleUpdateValidator)
    : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var users =
            await adminUserService
                .GetAllAsync();

        return Ok(users);
    }

    [HttpPut("{userId:guid}/role")]
    [Authorize(Roles = "SuperAdmin")]
    public async Task<IActionResult> SetRole(
        Guid userId,
        AdminUserRoleUpdateDto dto)
    {
        var validationResult =
            await roleUpdateValidator
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

        var actingUserId =
            User.GetUserId();

        await adminUserService.SetRoleAsync(
            actingUserId,
            userId,
            dto.Role);

        return NoContent();
    }
}
