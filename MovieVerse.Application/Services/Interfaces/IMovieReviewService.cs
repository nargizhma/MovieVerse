using MovieVerse.Dtos.Reviews;

namespace MovieVerse.Services.Interfaces;

public interface IMovieReviewService
{
    Task<List<ReviewReturnDto>> GetAllAsync(
        Guid movieId);

    Task<ReviewReturnDto> GetMineAsync(
        Guid movieId,
        Guid userId);

    Task CreateAsync(
        Guid movieId,
        Guid userId,
        ReviewCreateDto dto);

    Task UpdateAsync(
        Guid movieId,
        Guid userId,
        ReviewUpdateDto dto);

    Task DeleteMineAsync(
        Guid movieId,
        Guid userId);

    Task DeleteByAdminAsync(
        Guid movieId,
        Guid reviewId);
}
