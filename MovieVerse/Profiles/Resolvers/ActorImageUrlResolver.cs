using AutoMapper;
using MovieVerse.Dtos.Actors;
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
        if (string.IsNullOrWhiteSpace(source.ProfileImageUrl))
            return null;

        var request = httpContextAccessor.HttpContext?.Request;

        if (request is null)
            return $"/images/actors/{source.ProfileImageUrl}";

        return
            $"{request.Scheme}://{request.Host}/images/actors/{source.ProfileImageUrl}";
    }
}