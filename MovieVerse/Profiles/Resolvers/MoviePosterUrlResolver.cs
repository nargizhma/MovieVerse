using AutoMapper;
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
        if (string.IsNullOrWhiteSpace(source.PosterUrl))
            return null;

        var request =
            httpContextAccessor.HttpContext?.Request;

        if (request is null)
            return $"/images/movies/{source.PosterUrl}";

        return
            $"{request.Scheme}://{request.Host}/images/movies/{source.PosterUrl}";
    }
}