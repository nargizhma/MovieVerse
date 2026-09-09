using Microsoft.EntityFrameworkCore;
using MovieVerse.Data;
using MovieVerse.Models;
using MovieVerse.Repositories.Interfaces;

namespace MovieVerse.Repositories;

public class UserProfileRepository(
    AppDbContext context)
    : IUserProfileRepository
{
    public async Task<UserProfile?> GetByUserIdAsync(
        Guid userId,
        bool tracking = false)
    {
        IQueryable<UserProfile> query =
            context.UserProfiles;

        if (!tracking)
            query = query.AsNoTracking();

        return await query.FirstOrDefaultAsync(
            x => x.AppUserId == userId);
    }

    public async Task AddAsync(
        UserProfile profile)
    {
        await context.UserProfiles.AddAsync(
            profile);
    }
}