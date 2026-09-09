using AutoMapper;
using MovieVerse.Abstractions.Media;
using MovieVerse.Dtos.Directors;
using MovieVerse.Models;

namespace MovieVerse.Profiles.Resolvers;

public class DirectorImageUrlResolver(
    IMediaUrlBuilder mediaUrlBuilder)
    : IValueResolver<
        Director,
        DirectorReturnDto,
        string?>
{
    public string? Resolve(
        Director source,
        DirectorReturnDto destination,
        string? destMember,
        ResolutionContext context)
    {
        return mediaUrlBuilder.BuildImageUrl(
            source.ProfileImageUrl,
            "directors");
    }
}