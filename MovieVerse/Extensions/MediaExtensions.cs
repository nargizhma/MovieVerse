using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace MovieVerse.Extensions;

public static class MediaExtensions
{
    public static string GetImageFolderPath(
        this IWebHostEnvironment environment,
        string folder)
    {
        var webRootPath =
            environment.WebRootPath
            ?? Path.Combine(
                environment.ContentRootPath,
                "wwwroot");

        return Path.Combine(
            webRootPath,
            "images",
            folder);
    }

    public static string? BuildImageUrl(
        this IHttpContextAccessor httpContextAccessor,
        string? fileName,
        string folder)
    {
        if (string.IsNullOrWhiteSpace(fileName))
            return null;

        var relativeUrl =
            $"/images/{folder}/{fileName}";

        var request =
            httpContextAccessor
                .HttpContext?
                .Request;

        if (request is null)
            return relativeUrl;

        return
            $"{request.Scheme}://{request.Host}{relativeUrl}";
    }
}
