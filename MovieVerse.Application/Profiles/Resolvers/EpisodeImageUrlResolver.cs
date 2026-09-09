using AutoMapper;
using MovieVerse.Abstractions.Media;
using MovieVerse.Models;

namespace MovieVerse.Profiles.Resolvers;

public class EpisodeImageUrlResolver<TDestination>(
    IMediaUrlBuilder mediaUrlBuilder)
    : IValueResolver<
        Episode,
        TDestination,
        string?>
{
    public string? Resolve(
        Episode source,
        TDestination destination,
        string? destMember,
        ResolutionContext context)
    {
        return mediaUrlBuilder.BuildImageUrl(
            source.ImageUrl,
            "episodes");
    }
}