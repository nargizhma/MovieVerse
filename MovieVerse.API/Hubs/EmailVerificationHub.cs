using Microsoft.AspNetCore.SignalR;
using MovieVerse.Abstractions.Identity;

namespace MovieVerse.Hubs;

public class EmailVerificationHub(
    IIdentityService identityService)
    : Hub
{
    public async Task WatchVerification(
        string userId)
    {
        if (!Guid.TryParse(
                userId,
                out var parsedUserId))
        {
            throw new HubException(
                "Invalid user ID.");
        }

        var groupName =
            GetGroupName(
                parsedUserId);

        await Groups.AddToGroupAsync(
            Context.ConnectionId,
            groupName);

        // Prevent a race condition:
        // maybe verification happened before
        // SignalR finished connecting.
        var user =
            await identityService
                .FindByIdAsync(
                    parsedUserId);

        if (user?.EmailConfirmed == true)
        {
            await Clients.Caller
                .SendAsync(
                    "EmailVerified");
        }
    }

    public static string GetGroupName(
        Guid userId)
    {
        return
            $"email-verification:{userId}";
    }
}