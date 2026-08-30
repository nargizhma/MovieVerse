using System.Security.Claims;
using MovieVerse.Exceptions;

namespace MovieVerse.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static Guid GetUserId(
        this ClaimsPrincipal user)
    {
        var userIdValue =
            user.FindFirstValue(
                ClaimTypes.NameIdentifier);

        if (!Guid.TryParse(
                userIdValue,
                out var userId))
        {
            throw new UnauthorizedException(
                "User id could not be determined from the token.");
        }

        return userId;
    }
}
