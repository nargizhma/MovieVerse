namespace MovieVerse.Extensions;

public static class FileManager
{
    public static async Task<string> SaveFileAsync(
        this IFormFile file,
        string folderPath)
    {
        Directory.CreateDirectory(folderPath);

        var fileName =
            Guid.NewGuid() + Path.GetExtension(file.FileName);

        var path = Path.Combine(folderPath, fileName);

        await using var stream =
            new FileStream(path, FileMode.Create);

        await file.CopyToAsync(stream);

        return fileName;
    }

    public static bool IsImage(this IFormFile file)
    {
        return file.ContentType.StartsWith(
            "image/",
            StringComparison.OrdinalIgnoreCase);
    }

    public static bool IsValidSize(
        this IFormFile file,
        long maxMb)
    {
        return file.Length <= maxMb * 1024 * 1024;
    }

    public static void DeleteFile(
        string? fileName,
        string folderPath)
    {
        if (string.IsNullOrWhiteSpace(fileName))
            return;

        var path = Path.Combine(
            folderPath,
            fileName);

        if (File.Exists(path))
            File.Delete(path);
    }
}