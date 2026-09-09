using MovieVerse.Abstractions.Media;

namespace MovieVerse.Services;

public class HttpMediaUrlBuilder(
    IHttpContextAccessor httpContextAccessor)
    : IMediaUrlBuilder
{
    public string? BuildImageUrl(
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