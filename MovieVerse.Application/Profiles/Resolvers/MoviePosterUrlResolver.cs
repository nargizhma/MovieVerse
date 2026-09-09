using AutoMapper;
using MovieVerse.Abstractions.Media;
using MovieVerse.Models;

namespace MovieVerse.Profiles.Resolvers;

public class MoviePosterUrlResolver<TDestination>(
    IMediaUrlBuilder mediaUrlBuilder)
    : IValueResolver<
        Movie,
        TDestination,
        string?>
{
    public string? Resolve(
        Movie source,
        TDestination destination,
        string? destMember,
        ResolutionContext context)
    {
        return mediaUrlBuilder.BuildImageUrl(
            source.PosterUrl,
            "movies");
    }
}