using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MovieVerse.Dtos.Reviews;
using MovieVerse.Exceptions;
using MovieVerse.Models;
using MovieVerse.Repositories.Interfaces;
using MovieVerse.Services.Interfaces;

namespace MovieVerse.Services;

public class EpisodeReviewService(
    IGenericRepository<EpisodeReview> reviewRepository,
    IGenericRepository<Episode> episodeRepository,
    IMapper mapper)
    : IEpisodeReviewService
{
    public async Task<List<ReviewReturnDto>> GetAllAsync(
        Guid episodeId)
    {
        await EnsureEpisodeExistsAsync(episodeId);

        var reviews =
            await reviewRepository.Query()
                .Where(x => x.EpisodeId == episodeId)
                .Include(x => x.User)
                    .ThenInclude(x => x.Profile)
                .AsNoTracking()
                .ToListAsync();

        return mapper.Map<List<ReviewReturnDto>>(
            reviews);
    }

    public async Task<ReviewReturnDto> GetMineAsync(
        Guid episodeId,
        Guid userId)
    {
        await EnsureEpisodeExistsAsync(episodeId);

        var review =
            await reviewRepository.Query()
                .Include(x => x.User)
                    .ThenInclude(x => x.Profile)
                .AsNoTracking()
                .FirstOrDefaultAsync(x =>
                    x.EpisodeId == episodeId &&
                    x.UserId == userId);

        if (review is null)
            throw new NotFoundException(
                "You have not reviewed this episode yet.");

        return mapper.Map<ReviewReturnDto>(
            review);
    }

    public async Task CreateAsync(
        Guid episodeId,
        Guid userId,
        ReviewCreateDto dto)
    {
        await EnsureEpisodeExistsAsync(episodeId);

        var alreadyExists =
            await reviewRepository.Query()
                .AnyAsync(x =>
                    x.EpisodeId == episodeId &&
                    x.UserId == userId);

        if (alreadyExists)
            throw new AlreadyExistsException(
                "You have already rated or reviewed this episode.");

        var review =
            mapper.Map<EpisodeReview>(dto);

        review.EpisodeId = episodeId;
        review.UserId = userId;

        await reviewRepository.AddAsync(review);
        await reviewRepository.SaveChangesAsync();
    }

    public async Task UpdateAsync(
        Guid episodeId,
        Guid userId,
        ReviewUpdateDto dto)
    {
        await EnsureEpisodeExistsAsync(episodeId);

        var review =
            await reviewRepository.Query()
                .FirstOrDefaultAsync(x =>
                    x.EpisodeId == episodeId &&
                    x.UserId == userId);

        if (review is null)
            throw new NotFoundException(
                "You have not reviewed this episode yet.");

        mapper.Map(dto, review);

        reviewRepository.Update(review);
        await reviewRepository.SaveChangesAsync();
    }

    public async Task DeleteMineAsync(
        Guid episodeId,
        Guid userId)
    {
        await EnsureEpisodeExistsAsync(episodeId);

        var review =
            await reviewRepository.Query()
                .FirstOrDefaultAsync(x =>
                    x.EpisodeId == episodeId &&
                    x.UserId == userId);

        if (review is null)
            throw new NotFoundException(
                "You have not reviewed this episode yet.");

        reviewRepository.Delete(review);
        await reviewRepository.SaveChangesAsync();
    }

    public async Task DeleteByAdminAsync(
        Guid episodeId,
        Guid reviewId)
    {
        await EnsureEpisodeExistsAsync(episodeId);

        var review =
            await reviewRepository.Query()
                .FirstOrDefaultAsync(x =>
                    x.Id == reviewId &&
                    x.EpisodeId == episodeId);

        if (review is null)
            throw new NotFoundException(
                "Review was not found.");

        reviewRepository.Delete(review);
        await reviewRepository.SaveChangesAsync();
    }

    private async Task EnsureEpisodeExistsAsync(
        Guid episodeId)
    {
        var exists =
            await episodeRepository.Query()
                .AnyAsync(x => x.Id == episodeId);

        if (!exists)
            throw new NotFoundException(
                "Episode was not found.");
    }
}
