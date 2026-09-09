using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MovieVerse.Models;
using MovieVerse.Services.Interfaces;
using MovieVerse.Settings;

namespace MovieVerse.Services;

public class JwtService(
    IOptions<JwtSettings> jwtOptions,
    UserManager<AppUser> userManager)
    : IJwtService
{
    public async Task<string> CreateTokenAsync(
        Guid userId)
    {
        var user =
            await userManager.FindByIdAsync(
                userId.ToString());

        if (user is null)
            throw new InvalidOperationException(
                "User could not be found while creating JWT.");

        var jwtSettings =
            jwtOptions.Value;

        var claims =
            new List<Claim>
            {
                new(
                    JwtRegisteredClaimNames.Sub,
                    user.Id.ToString()),

                new(
                    ClaimTypes.NameIdentifier,
                    user.Id.ToString()),

                new(
                    ClaimTypes.Name,
                    user.UserName
                    ?? string.Empty),

                new(
                    JwtRegisteredClaimNames.Jti,
                    Guid.NewGuid()
                        .ToString())
            };

        if (!string.IsNullOrWhiteSpace(
                user.Email))
        {
            claims.Add(
                new Claim(
                    JwtRegisteredClaimNames.Email,
                    user.Email));
        }

        var roles =
            await userManager.GetRolesAsync(
                user);

        foreach (var role in roles)
        {
            claims.Add(
                new Claim(
                    ClaimTypes.Role,
                    role));
        }

        var key =
            new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(
                    jwtSettings.Key));

        var credentials =
            new SigningCredentials(
                key,
                SecurityAlgorithms.HmacSha256);

        var token =
            new JwtSecurityToken(
                issuer:
                    jwtSettings.Issuer,

                audience:
                    jwtSettings.Audience,

                claims:
                    claims,

                expires:
                    DateTime.UtcNow
                        .AddMinutes(
                            jwtSettings
                                .ExpirationMinutes),

                signingCredentials:
                    credentials);

        return new JwtSecurityTokenHandler()
            .WriteToken(token);
    }
}