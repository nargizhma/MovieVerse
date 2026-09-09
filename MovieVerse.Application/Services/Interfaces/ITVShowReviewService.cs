using MovieVerse.Dtos.Reviews;

namespace MovieVerse.Services.Interfaces;

public interface ITVShowReviewService
{
    Task<List<ReviewReturnDto>> GetAllAsync(
        Guid tvShowId);

    Task<ReviewReturnDto> GetMineAsync(
        Guid tvShowId,
        Guid userId);

    Task CreateAsync(
        Guid tvShowId,
        Guid userId,
        ReviewCreateDto dto);

    Task UpdateAsync(
        Guid tvShowId,
        Guid userId,
        ReviewUpdateDto dto);

    Task DeleteMineAsync(
        Guid tvShowId,
        Guid userId);

    Task DeleteByAdminAsync(
        Guid tvShowId,
        Guid reviewId);
}
