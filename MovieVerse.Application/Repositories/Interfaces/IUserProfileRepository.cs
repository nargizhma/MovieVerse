using MovieVerse.Models;

namespace MovieVerse.Repositories.Interfaces;

public interface IUserProfileRepository
{
    Task<UserProfile?> GetByUserIdAsync(
        Guid userId,
        bool tracking = false);

    Task AddAsync(UserProfile profile);
}