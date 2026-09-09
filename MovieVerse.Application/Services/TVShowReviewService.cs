using AutoMapper;
using MovieVerse.Abstractions.Identity;
using MovieVerse.Dtos.Reviews;
using MovieVerse.Exceptions;
using MovieVerse.Models;
using MovieVerse.Repositories.Interfaces;
using MovieVerse.Services.Interfaces;

namespace MovieVerse.Services;

public class TVShowReviewService(
    IGenericRepository<TVShowReview> reviewRepository,
    IGenericRepository<TVShow> tvShowRepository,
    IIdentityService identityService,
    IMapper mapper)
    : ITVShowReviewService
{
    public async Task<List<ReviewReturnDto>>
        GetAllAsync(Guid tvShowId)
    {
        await EnsureTVShowExistsAsync(tvShowId);

        var reviews =
            await reviewRepository.FindAllAsync(
                x => x.TVShowId == tvShowId);

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
            Guid tvShowId,
            Guid userId)
    {
        await EnsureTVShowExistsAsync(tvShowId);

        var review =
            await reviewRepository.FirstOrDefaultAsync(
                x =>
                    x.TVShowId == tvShowId &&
                    x.UserId == userId);

        if (review is null)
            throw new NotFoundException(
                "You have not reviewed this TV show yet.");

        var user =
            await identityService.FindByIdAsync(
                userId);

        return CreateReturnDto(
            review,
            user);
    }

    public async Task CreateAsync(
        Guid tvShowId,
        Guid userId,
        ReviewCreateDto dto)
    {
        await EnsureTVShowExistsAsync(tvShowId);

        var alreadyExists =
            await reviewRepository.AnyAsync(
                x =>
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
            await reviewRepository.FirstOrDefaultAsync(
                x =>
                    x.TVShowId == tvShowId &&
                    x.UserId == userId,
                true);

        if (review is null)
            throw new NotFoundException(
                "You have not reviewed this TV show yet.");

        mapper.Map(dto, review);

        review.UpdatedAt = DateTime.UtcNow;

        reviewRepository.Update(review);

        await reviewRepository.SaveChangesAsync();
    }

    public async Task DeleteMineAsync(
        Guid tvShowId,
        Guid userId)
    {
        await EnsureTVShowExistsAsync(tvShowId);

        var review =
            await reviewRepository.FirstOrDefaultAsync(
                x =>
                    x.TVShowId == tvShowId &&
                    x.UserId == userId,
                true);

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
            await reviewRepository.FirstOrDefaultAsync(
                x =>
                    x.Id == reviewId &&
                    x.TVShowId == tvShowId,
                true);

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
            await tvShowRepository.AnyAsync(
                x => x.Id == tvShowId);

        if (!exists)
            throw new NotFoundException(
                "TV show was not found.");
    }

    private static ReviewReturnDto CreateReturnDto(
        TVShowReview review,
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