using AutoMapper;
using MovieVerse.Dtos.Actors;
using MovieVerse.Extensions;
using MovieVerse.Models;

namespace MovieVerse.Profiles.Resolvers;

public class ActorImageUrlResolver(
    IHttpContextAccessor httpContextAccessor)
    : IValueResolver<Actor, ActorReturnDto, string?>
{
    public string? Resolve(
        Actor source,
        ActorReturnDto destination,
        string? destMember,
        ResolutionContext context)
    {
        return httpContextAccessor.BuildImageUrl(
            source.ProfileImageUrl,
            "actors");
    }
}
