using FluentValidation;
using MovieVerse.Dtos.Admin;

namespace MovieVerse.Validators.Admin;

public class AdminUserRoleUpdateDtoValidator
    : AbstractValidator<AdminUserRoleUpdateDto>
{
    private static readonly string[] AllowedRoles =
    [
        "User",
        "Admin",
        "SuperAdmin"
    ];

    public AdminUserRoleUpdateDtoValidator()
    {
        RuleFor(x => x.Role)
            .NotEmpty()
            .WithMessage(
                "Role is required.")
            .Must(role =>
                AllowedRoles.Contains(
                    role,
                    StringComparer.OrdinalIgnoreCase))
            .WithMessage(
                "Role must be User, Admin, or SuperAdmin.");
    }
}
