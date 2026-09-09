namespace MovieVerse.Extensions;

public static class FileManager
{
    public static bool IsImage(
        this IFormFile file)
    {
        return file.ContentType.StartsWith(
            "image/",
            StringComparison.OrdinalIgnoreCase);
    }

    public static bool IsValidSize(
        this IFormFile file,
        long maxMb)
    {
        return file.Length <=
               maxMb * 1024 * 1024;
    }
}