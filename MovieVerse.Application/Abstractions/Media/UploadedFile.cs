namespace MovieVerse.Abstractions.Media;

public class UploadedFile
{
    public string FileName { get; init; } = null!;

    public string ContentType { get; init; } = null!;

    public long Length { get; init; }

    public Stream Content { get; init; } = null!;
}