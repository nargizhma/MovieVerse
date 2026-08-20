using MovieVerse.Models;

namespace MovieVerse.Services.Interfaces;

public interface IJwtService
{
    Task<string> CreateTokenAsync(AppUser user);
}