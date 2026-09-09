namespace MovieVerse.Abstractions.Media;

public interface IMediaUrlBuilder
{
    string? BuildImageUrl(
        string? fileName,
        string folder);
}