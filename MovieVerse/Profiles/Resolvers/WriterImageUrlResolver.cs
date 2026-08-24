using AutoMapper;
using MovieVerse.Dtos.Writers;
using MovieVerse.Models;

namespace MovieVerse.Profiles.Resolvers;

public class WriterImageUrlResolver(
    IHttpContextAccessor httpContextAccessor)
    : IValueResolver<Writer, WriterReturnDto, string?>
{
    public string? Resolve(
        Writer source,
        WriterReturnDto destination,
        string? destMember,
        ResolutionContext context)
    {
        if (string.IsNullOrWhiteSpace(source.ProfileImageUrl))
            return null;

        var request =
            httpContextAccessor.HttpContext?.Request;

        if (request is null)
            return $"/images/writers/{source.ProfileImageUrl}";

        return
            $"{request.Scheme}://{request.Host}/images/writers/{source.ProfileImageUrl}";
    }
}