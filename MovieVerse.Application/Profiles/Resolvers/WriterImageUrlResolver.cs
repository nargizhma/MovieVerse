using AutoMapper;
using MovieVerse.Abstractions.Media;
using MovieVerse.Dtos.Writers;
using MovieVerse.Models;

namespace MovieVerse.Profiles.Resolvers;

public class WriterImageUrlResolver(
    IMediaUrlBuilder mediaUrlBuilder)
    : IValueResolver<
        Writer,
        WriterReturnDto,
        string?>
{
    public string? Resolve(
        Writer source,
        WriterReturnDto destination,
        string? destMember,
        ResolutionContext context)
    {
        return mediaUrlBuilder.BuildImageUrl(
            source.ProfileImageUrl,
            "writers");
    }
}