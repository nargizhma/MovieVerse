using Microsoft.AspNetCore.Identity;
using MovieVerse.Models;

namespace MovieVerse.Data.Seed;

public static class IdentitySeeder
{
    public static async Task SeedAsync(
        IServiceProvider serviceProvider)
    {
        var roleManager =
            serviceProvider.GetRequiredService<
                RoleManager<IdentityRole<Guid>>>();

        var userManager =
            serviceProvider.GetRequiredService<
                UserManager<AppUser>>();

        var configuration =
            serviceProvider.GetRequiredService<
                IConfiguration>();

        string[] roles =
        {
            "SuperAdmin",
            "Admin",
            "User"
        };

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                var result =
                    await roleManager.CreateAsync(
                        new IdentityRole<Guid>(role));

                if (!result.Succeeded)
                {
                    throw new InvalidOperationException(
                        string.Join(
                            " ",
                            result.Errors.Select(x =>
                                x.Description)));
                }
            }
        }

        var email =
            configuration[
                "Seed:SuperAdmin:Email"];

        var userName =
            configuration[
                "Seed:SuperAdmin:UserName"];

        var password =
            configuration[
                "Seed:SuperAdmin:Password"];

        if (string.IsNullOrWhiteSpace(email) ||
            string.IsNullOrWhiteSpace(userName) ||
            string.IsNullOrWhiteSpace(password))
        {
            return;
        }

        var superAdmin =
            await userManager.FindByEmailAsync(
                email);

        if (superAdmin is null)
        {
            superAdmin = new AppUser
            {
                Email = email,
                UserName = userName,
                EmailConfirmed = true
            };

            var createResult =
                await userManager.CreateAsync(
                    superAdmin,
                    password);

            if (!createResult.Succeeded)
            {
                throw new InvalidOperationException(
                    string.Join(
                        " ",
                        createResult.Errors.Select(x =>
                            x.Description)));
            }
        }

        var currentRoles =
            await userManager.GetRolesAsync(
                superAdmin);

        if (currentRoles.Count > 0 &&
            !(currentRoles.Count == 1 &&
              currentRoles.Contains(
                  "SuperAdmin")))
        {
            var removeResult =
                await userManager.RemoveFromRolesAsync(
                    superAdmin,
                    currentRoles);

            if (!removeResult.Succeeded)
            {
                throw new InvalidOperationException(
                    string.Join(
                        " ",
                        removeResult.Errors.Select(x =>
                            x.Description)));
            }
        }

        if (!await userManager.IsInRoleAsync(
                superAdmin,
                "SuperAdmin"))
        {
            var roleResult =
                await userManager.AddToRoleAsync(
                    superAdmin,
                    "SuperAdmin");

            if (!roleResult.Succeeded)
            {
                throw new InvalidOperationException(
                    string.Join(
                        " ",
                        roleResult.Errors.Select(x =>
                            x.Description)));
            }
        }
    }
}