using AutoMapper;
using MovieVerse.Dtos.Directors;
using MovieVerse.Extensions;
using MovieVerse.Models;

namespace MovieVerse.Profiles.Resolvers;

public class DirectorImageUrlResolver(
    IHttpContextAccessor httpContextAccessor)
    : IValueResolver<Director, DirectorReturnDto, string?>
{
    public string? Resolve(
        Director source,
        DirectorReturnDto destination,
        string? destMember,
        ResolutionContext context)
    {
        return httpContextAccessor.BuildImageUrl(
            source.ProfileImageUrl,
            "directors");
    }
}
