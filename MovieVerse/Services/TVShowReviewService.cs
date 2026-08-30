using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MovieVerse.Dtos.Reviews;
using MovieVerse.Exceptions;
using MovieVerse.Models;
using MovieVerse.Repositories.Interfaces;
using MovieVerse.Services.Interfaces;

namespace MovieVerse.Services;

public class TVShowReviewService(
    IGenericRepository<TVShowReview> reviewRepository,
    IGenericRepository<TVShow> tvShowRepository,
    IMapper mapper)
    : ITVShowReviewService
{
    public async Task<List<ReviewReturnDto>> GetAllAsync(
        Guid tvShowId)
    {
        await EnsureTVShowExistsAsync(tvShowId);

        var reviews =
            await reviewRepository.Query()
                .Where(x => x.TVShowId == tvShowId)
                .Include(x => x.User)
                    .ThenInclude(x => x.Profile)
                .AsNoTracking()
                .ToListAsync();

        return mapper.Map<List<ReviewReturnDto>>(
            reviews);
    }

    public async Task<ReviewReturnDto> GetMineAsync(
        Guid tvShowId,
        Guid userId)
    {
        await EnsureTVShowExistsAsync(tvShowId);

        var review =
            await reviewRepository.Query()
                .Include(x => x.User)
                    .ThenInclude(x => x.Profile)
                .AsNoTracking()
                .FirstOrDefaultAsync(x =>
                    x.TVShowId == tvShowId &&
                    x.UserId == userId);

        if (review is null)
            throw new NotFoundException(
                "You have not reviewed this TV show yet.");

        return mapper.Map<ReviewReturnDto>(
            review);
    }

    public async Task CreateAsync(
        Guid tvShowId,
        Guid userId,
        ReviewCreateDto dto)
    {
        await EnsureTVShowExistsAsync(tvShowId);

        var alreadyExists =
            await reviewRepository.Query()
                .AnyAsync(x =>
                    x.TVShowId == tvShowId &&
                    x.UserId == userId);

        if (alreadyExists)
            throw new AlreadyExistsException(
                "You have already rated or reviewed this TV show.");

        var review =
            mapper.Map<TVShowReview>(dto);

        review.TVShowId = tvShowId;
        review.UserId = userId;

        await reviewRepository.AddAsync(review);
        await reviewRepository.SaveChangesAsync();
    }

    public async Task UpdateAsync(
        Guid tvShowId,
        Guid userId,
        ReviewUpdateDto dto)
    {
        await EnsureTVShowExistsAsync(tvShowId);

        var review =
            await reviewRepository.Query()
                .FirstOrDefaultAsync(x =>
                    x.TVShowId == tvShowId &&
                    x.UserId == userId);

        if (review is null)
            throw new NotFoundException(
                "You have not reviewed this TV show yet.");

        mapper.Map(dto, review);

        reviewRepository.Update(review);
        await reviewRepository.SaveChangesAsync();
    }

    public async Task DeleteMineAsync(
        Guid tvShowId,
        Guid userId)
    {
        await EnsureTVShowExistsAsync(tvShowId);

        var review =
            await reviewRepository.Query()
                .FirstOrDefaultAsync(x =>
                    x.TVShowId == tvShowId &&
                    x.UserId == userId);

        if (review is null)
            throw new NotFoundException(
                "You have not reviewed this TV show yet.");

        reviewRepository.Delete(review);
        await reviewRepository.SaveChangesAsync();
    }

    public async Task DeleteByAdminAsync(
        Guid tvShowId,
        Guid reviewId)
    {
        await EnsureTVShowExistsAsync(tvShowId);

        var review =
            await reviewRepository.Query()
                .FirstOrDefaultAsync(x =>
                    x.Id == reviewId &&
                    x.TVShowId == tvShowId);

        if (review is null)
            throw new NotFoundException(
                "Review was not found.");

        reviewRepository.Delete(review);
        await reviewRepository.SaveChangesAsync();
    }

    private async Task EnsureTVShowExistsAsync(
        Guid tvShowId)
    {
        var exists =
            await tvShowRepository.Query()
                .AnyAsync(x => x.Id == tvShowId);

        if (!exists)
            throw new NotFoundException(
                "TV show was not found.");
    }
}
