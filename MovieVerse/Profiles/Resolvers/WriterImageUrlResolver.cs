using AutoMapper;
using MovieVerse.Dtos.Writers;
using MovieVerse.Extensions;
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
        return httpContextAccessor.BuildImageUrl(
            source.ProfileImageUrl,
            "writers");
    }
}
