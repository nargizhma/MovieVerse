using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MovieVerse.Abstractions.Identity;
using MovieVerse.Data;
using MovieVerse.Models;

namespace MovieVerse.Infrastructure.Identity;

public class IdentityService(
    AppDbContext dbContext,
    UserManager<AppUser> userManager)
    : IIdentityService
{
    public async Task<IdentityUserInfo?>
        FindByIdAsync(Guid userId)
    {
        var user =
            await dbContext.Users
                .Include(x => x.Profile)
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    x => x.Id == userId);

        if (user is null)
            return null;

        return await MapAsync(user);
    }

    public async Task<IdentityUserInfo?>
        FindByEmailAsync(string email)
    {
        var user =
            await userManager
                .FindByEmailAsync(email);

        if (user is null)
            return null;

        return await MapAsync(user);
    }

    public async Task<IdentityUserInfo?>
        FindByUserNameAsync(
            string userName)
    {
        var normalized =
            userName.Trim();

        var user =
            await dbContext.Users
                .Include(x => x.Profile)
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    x => x.UserName ==
                         normalized);

        if (user is null)
            return null;

        return await MapAsync(user);
    }

    public async Task<List<IdentityUserInfo>>
        GetAllUsersAsync()
    {
        var users =
            await dbContext.Users
                .Include(x => x.Profile)
                .AsNoTracking()
                .OrderBy(x => x.UserName)
                .ToListAsync();

        var result =
            new List<IdentityUserInfo>();

        foreach (var user in users)
        {
            result.Add(
                await MapAsync(user));
        }

        return result;
    }

    public async Task<IdentityOperationResult>
        CreateAsync(
            string email,
            string userName,
            string password)
    {
        var user =
            new AppUser
            {
                Email = email,
                UserName = userName
            };

        var result =
            await userManager.CreateAsync(
                user,
                password);

        return new IdentityOperationResult
        {
            Succeeded = result.Succeeded,

            UserId =
                result.Succeeded
                    ? user.Id
                    : null,

            Errors =
                result.Errors
                    .Select(x =>
                        x.Description)
                    .ToList()
        };
    }

    public async Task<bool>
        CheckPasswordAsync(
            Guid userId,
            string password)
    {
        var user =
            await userManager.FindByIdAsync(
                userId.ToString());

        if (user is null)
            return false;

        return await userManager
            .CheckPasswordAsync(
                user,
                password);
    }

    public async Task<IReadOnlyList<string>>
        GetRolesAsync(Guid userId)
    {
        var user =
            await userManager.FindByIdAsync(
                userId.ToString());

        if (user is null)
            return [];

        var roles =
            await userManager.GetRolesAsync(
                user);

        return roles.ToList();
    }

    public async Task<IdentityOperationResult>
        AddToRoleAsync(
            Guid userId,
            string role)
    {
        var user =
            await userManager.FindByIdAsync(
                userId.ToString());

        if (user is null)
        {
            return new IdentityOperationResult
            {
                Succeeded = false,
                Errors =
                [
                    "User was not found."
                ]
            };
        }

        var result =
            await userManager.AddToRoleAsync(
                user,
                role);

        return FromIdentityResult(
            result,
            user.Id);
    }

    public async Task<IdentityOperationResult>
        RemoveFromRolesAsync(
            Guid userId,
            IEnumerable<string> roles)
    {
        var user =
            await userManager.FindByIdAsync(
                userId.ToString());

        if (user is null)
        {
            return new IdentityOperationResult
            {
                Succeeded = false,
                Errors =
                [
                    "User was not found."
                ]
            };
        }

        var result =
            await userManager
                .RemoveFromRolesAsync(
                    user,
                    roles);

        return FromIdentityResult(
            result,
            user.Id);
    }

    public async Task<int> CountUsersAsync()
    {
        return await dbContext.Users
            .CountAsync();
    }

    public async Task<int> CountUsersInRoleAsync(
        string role)
    {
        var users =
            await userManager
                .GetUsersInRoleAsync(role);

        return users.Count;
    }

    private async Task<IdentityUserInfo>
        MapAsync(AppUser user)
    {
        var roles =
            await userManager.GetRolesAsync(
                user);

        UserProfile? profile =
            user.Profile;

        if (profile is null)
        {
            profile =
                await dbContext.UserProfiles
                    .AsNoTracking()
                    .FirstOrDefaultAsync(
                        x =>
                            x.AppUserId ==
                            user.Id);
        }

        return new IdentityUserInfo
        {
            Id = user.Id,

            UserName =
                user.UserName
                ?? string.Empty,

            Email =
                user.Email
                ?? string.Empty,

            DisplayName =
                profile?.DisplayName,

            Bio =
                profile?.Bio,

            ProfileImageUrl =
                profile?.ProfileImageUrl,

            Roles =
                roles
                    .OrderBy(x => x)
                    .ToList(),
            EmailConfirmed =
            user.EmailConfirmed
        };
    }

    private static IdentityOperationResult
        FromIdentityResult(
            IdentityResult result,
            Guid userId)
    {
        return new IdentityOperationResult
        {
            Succeeded =
                result.Succeeded,

            UserId =
                result.Succeeded
                    ? userId
                    : null,

            Errors =
                result.Errors
                    .Select(x =>
                        x.Description)
                    .ToList()
        };
    }
    public async Task<string?>
        GenerateEmailConfirmationTokenAsync(
            Guid userId)
    {
        var user =
            await userManager.FindByIdAsync(
                userId.ToString());

        if (user is null)
            return null;

        return await userManager
            .GenerateEmailConfirmationTokenAsync(
                user);
    }
    public async Task<IdentityOperationResult>
        ConfirmEmailAsync(
            Guid userId,
            string token)
    {
        var user =
            await userManager.FindByIdAsync(
                userId.ToString());

        if (user is null)
        {
            return new IdentityOperationResult
            {
                Succeeded = false,
                Errors =
                [
                    "User was not found."
                ]
            };
        }

        var result =
            await userManager
                .ConfirmEmailAsync(
                    user,
                    token);

        return FromIdentityResult(
            result,
            user.Id);
    }
}