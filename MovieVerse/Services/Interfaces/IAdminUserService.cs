using MovieVerse.Dtos.Admin;

namespace MovieVerse.Services.Interfaces;

public interface IAdminUserService
{
    Task<List<AdminUserReturnDto>> GetAllAsync();

    Task SetRoleAsync(
        Guid actingUserId,
        Guid targetUserId,
        string role);
}
