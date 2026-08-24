using AutoMapper;
using MovieVerse.Dtos.Directors;
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
        if (string.IsNullOrWhiteSpace(source.ProfileImageUrl))
            return null;

        var request =
            httpContextAccessor.HttpContext?.Request;

        if (request is null)
            return $"/images/directors/{source.ProfileImageUrl}";

        return
            $"{request.Scheme}://{request.Host}/images/directors/{source.ProfileImageUrl}";
    }
}