using AutoMapper;
using MovieVerse.Models;

namespace MovieVerse.Profiles.Resolvers;

public class EpisodeImageUrlResolver<TDestination>(
    IHttpContextAccessor httpContextAccessor)
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
        if (string.IsNullOrWhiteSpace(
                source.ImageUrl))
            return null;

        var request =
            httpContextAccessor
                .HttpContext?
                .Request;

        if (request is null)
            return
                $"/images/episodes/{source.ImageUrl}";

        return
            $"{request.Scheme}://{request.Host}/images/episodes/{source.ImageUrl}";
    }
}