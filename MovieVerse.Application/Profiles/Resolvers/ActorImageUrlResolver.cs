using AutoMapper;
using MovieVerse.Abstractions.Media;
using MovieVerse.Dtos.Actors;
using MovieVerse.Models;

namespace MovieVerse.Profiles.Resolvers;

public class ActorImageUrlResolver(
    IMediaUrlBuilder mediaUrlBuilder)
    : IValueResolver<
        Actor,
        ActorReturnDto,
        string?>
{
    public string? Resolve(
        Actor source,
        ActorReturnDto destination,
        string? destMember,
        ResolutionContext context)
    {
        return mediaUrlBuilder.BuildImageUrl(
            source.ProfileImageUrl,
            "actors");
    }
}