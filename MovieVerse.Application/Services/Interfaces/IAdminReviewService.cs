using MovieVerse.Dtos.Admin;

namespace MovieVerse.Services.Interfaces;

public interface IAdminReviewService
{
    Task<List<AdminReviewReturnDto>> GetAllAsync(
        string? type);

    Task DeleteAsync(
        string type,
        Guid reviewId);
}
