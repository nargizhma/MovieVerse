using AutoMapper;
using MovieVerse.Models;

namespace MovieVerse.Profiles.Resolvers;

public class TVShowPosterUrlResolver<TDestination>(
    IHttpContextAccessor httpContextAccessor)
    : IValueResolver<
        TVShow,
        TDestination,
        string?>
{
    public string? Resolve(
        TVShow source,
        TDestination destination,
        string? destMember,
        ResolutionContext context)
    {
        if (string.IsNullOrWhiteSpace(
                source.PosterUrl))
            return null;

        var request =
            httpContextAccessor
                .HttpContext?
                .Request;

        if (request is null)
            return
                $"/images/tvshows/{source.PosterUrl}";

        return
            $"{request.Scheme}://{request.Host}/images/tvshows/{source.PosterUrl}";
    }
}