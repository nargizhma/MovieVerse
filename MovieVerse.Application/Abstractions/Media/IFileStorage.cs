namespace MovieVerse.Abstractions.Media;

public interface IFileStorage
{
    Task<string> SaveAsync(
        UploadedFile file,
        string folder);

    void Delete(
        string? fileName,
        string folder);
}