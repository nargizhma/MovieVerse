using AutoMapper;
using MovieVerse.Abstractions.Media;
using MovieVerse.Models;

namespace MovieVerse.Profiles.Resolvers;

public class TVShowPosterUrlResolver<TDestination>(
    IMediaUrlBuilder mediaUrlBuilder)
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
        return mediaUrlBuilder.BuildImageUrl(
            source.PosterUrl,
            "tvshows");
    }
}