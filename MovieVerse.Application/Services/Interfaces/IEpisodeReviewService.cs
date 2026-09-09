using MovieVerse.Dtos.Reviews;

namespace MovieVerse.Services.Interfaces;

public interface IEpisodeReviewService
{
    Task<List<ReviewReturnDto>> GetAllAsync(
        Guid episodeId);

    Task<ReviewReturnDto> GetMineAsync(
        Guid episodeId,
        Guid userId);

    Task CreateAsync(
        Guid episodeId,
        Guid userId,
        ReviewCreateDto dto);

    Task UpdateAsync(
        Guid episodeId,
        Guid userId,
        ReviewUpdateDto dto);

    Task DeleteMineAsync(
        Guid episodeId,
        Guid userId);

    Task DeleteByAdminAsync(
        Guid episodeId,
        Guid reviewId);
}
