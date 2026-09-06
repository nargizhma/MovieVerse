using AutoMapper;
using MovieVerse.Extensions;
using MovieVerse.Models;

namespace MovieVerse.Profiles.Resolvers;

public class MoviePosterUrlResolver<TDestination>(
    IHttpContextAccessor httpContextAccessor)
    : IValueResolver<Movie, TDestination, string?>
{
    public string? Resolve(
        Movie source,
        TDestination destination,
        string? destMember,
        ResolutionContext context)
    {
        return httpContextAccessor.BuildImageUrl(
            source.PosterUrl,
            "movies");
    }
}
