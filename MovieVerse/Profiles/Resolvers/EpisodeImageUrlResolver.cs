using AutoMapper;
using MovieVerse.Extensions;
using MovieVerse.Models;

namespace MovieVerse.Profiles.Resolvers;

public class EpisodeImageUrlResolver<TDestination>(
    IHttpContextAccessor httpContextAccessor)
    : IValueResolver<Episode, TDestination, string?>
{
    public string? Resolve(
        Episode source,
        TDestination destination,
        string? destMember,
        ResolutionContext context)
    {
        return httpContextAccessor.BuildImageUrl(
            source.ImageUrl,
            "episodes");
    }
}
