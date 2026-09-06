using AutoMapper;
using MovieVerse.Extensions;
using MovieVerse.Models;

namespace MovieVerse.Profiles.Resolvers;

public class TVShowPosterUrlResolver<TDestination>(
    IHttpContextAccessor httpContextAccessor)
    : IValueResolver<TVShow, TDestination, string?>
{
    public string? Resolve(
        TVShow source,
        TDestination destination,
        string? destMember,
        ResolutionContext context)
    {
        return httpContextAccessor.BuildImageUrl(
            source.PosterUrl,
            "tvshows");
    }
}
