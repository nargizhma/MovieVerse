using MovieVerse.Abstractions.Media;

namespace MovieVerse.Infrastructure.Media;

public class LocalFileStorage(
    string webRootPath)
    : IFileStorage
{
    public async Task<string> SaveAsync(
        UploadedFile file,
        string folder)
    {
        var folderPath =
            Path.Combine(
                webRootPath,
                "images",
                folder);

        Directory.CreateDirectory(
            folderPath);

        var fileName =
            Guid.NewGuid() +
            Path.GetExtension(
                file.FileName);

        var path =
            Path.Combine(
                folderPath,
                fileName);

        await using var output =
            new FileStream(
                path,
                FileMode.Create);

        await using var input =
            file.Content;

        await input.CopyToAsync(
            output);

        return fileName;
    }

    public void Delete(
        string? fileName,
        string folder)
    {
        if (string.IsNullOrWhiteSpace(
                fileName))
            return;

        var path =
            Path.Combine(
                webRootPath,
                "images",
                folder,
                fileName);

        if (File.Exists(path))
            File.Delete(path);
    }
}