using AutoMapper;
using MovieVerse.Abstractions.Identity;
using MovieVerse.Dtos.Reviews;
using MovieVerse.Exceptions;
using MovieVerse.Models;
using MovieVerse.Repositories.Interfaces;
using MovieVerse.Services.Interfaces;

namespace MovieVerse.Services;

public class EpisodeReviewService(
    IGenericRepository<EpisodeReview> reviewRepository,
    IGenericRepository<Episode> episodeRepository,
    IIdentityService identityService,
    IMapper mapper)
    : IEpisodeReviewService
{
    public async Task<List<ReviewReturnDto>>
        GetAllAsync(Guid episodeId)
    {
        await EnsureEpisodeExistsAsync(episodeId);

        var reviews =
            await reviewRepository.FindAllAsync(
                x => x.EpisodeId == episodeId);

        reviews = reviews
            .OrderByDescending(x => x.CreatedAt)
            .ToList();

        var users =
            (await identityService.GetAllUsersAsync())
            .ToDictionary(x => x.Id);

        return reviews
            .Select(review =>
            {
                users.TryGetValue(
                    review.UserId,
                    out var user);

                return CreateReturnDto(
                    review,
                    user);
            })
            .ToList();
    }

    public async Task<ReviewReturnDto>
        GetMineAsync(
            Guid episodeId,
            Guid userId)
    {
        await EnsureEpisodeExistsAsync(episodeId);

        var review =
            await reviewRepository.FirstOrDefaultAsync(
                x =>
                    x.EpisodeId == episodeId &&
                    x.UserId == userId);

        if (review is null)
            throw new NotFoundException(
                "You have not reviewed this episode yet.");

        var user =
            await identityService.FindByIdAsync(
                userId);

        return CreateReturnDto(
            review,
            user);
    }

    public async Task CreateAsync(
        Guid episodeId,
        Guid userId,
        ReviewCreateDto dto)
    {
        await EnsureEpisodeExistsAsync(episodeId);

        var alreadyExists =
            await reviewRepository.AnyAsync(
                x =>
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
            await reviewRepository.FirstOrDefaultAsync(
                x =>
                    x.EpisodeId == episodeId &&
                    x.UserId == userId,
                true);

        if (review is null)
            throw new NotFoundException(
                "You have not reviewed this episode yet.");

        mapper.Map(dto, review);

        review.UpdatedAt = DateTime.UtcNow;

        reviewRepository.Update(review);

        await reviewRepository.SaveChangesAsync();
    }

    public async Task DeleteMineAsync(
        Guid episodeId,
        Guid userId)
    {
        await EnsureEpisodeExistsAsync(episodeId);

        var review =
            await reviewRepository.FirstOrDefaultAsync(
                x =>
                    x.EpisodeId == episodeId &&
                    x.UserId == userId,
                true);

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
            await reviewRepository.FirstOrDefaultAsync(
                x =>
                    x.Id == reviewId &&
                    x.EpisodeId == episodeId,
                true);

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
            await episodeRepository.AnyAsync(
                x => x.Id == episodeId);

        if (!exists)
            throw new NotFoundException(
                "Episode was not found.");
    }

    private static ReviewReturnDto CreateReturnDto(
        EpisodeReview review,
        IdentityUserInfo? user)
    {
        return new ReviewReturnDto
        {
            Id = review.Id,
            UserId = review.UserId,
            UserName =
                user?.UserName
                ?? string.Empty,
            DisplayName =
                user?.DisplayName,
            Rating = review.Rating,
            Content = review.Content,
            CreatedAt = review.CreatedAt,
            UpdatedAt = review.UpdatedAt
        };
    }
}