using MovieVerse.Dtos.Admin;

namespace MovieVerse.Services.Interfaces;

public interface IAdminDashboardService
{
    Task<AdminDashboardStatsDto> GetStatsAsync();
}
