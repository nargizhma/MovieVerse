namespace MovieVerse.Services.Interfaces;

public interface IJwtService
{
    Task<string> CreateTokenAsync(
        Guid userId);
}