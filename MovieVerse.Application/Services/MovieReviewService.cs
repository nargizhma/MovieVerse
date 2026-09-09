using AutoMapper;
using MovieVerse.Abstractions.Identity;
using MovieVerse.Dtos.Reviews;
using MovieVerse.Exceptions;
using MovieVerse.Models;
using MovieVerse.Repositories.Interfaces;
using MovieVerse.Services.Interfaces;

namespace MovieVerse.Services;

public class MovieReviewService(
    IGenericRepository<MovieReview> reviewRepository,
    IGenericRepository<Movie> movieRepository,
    IIdentityService identityService,
    IMapper mapper)
    : IMovieReviewService
{
    public async Task<List<ReviewReturnDto>>
        GetAllAsync(Guid movieId)
    {
        await EnsureMovieExistsAsync(movieId);

        var reviews =
            await reviewRepository.FindAllAsync(
                x => x.MovieId == movieId);

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
            Guid movieId,
            Guid userId)
    {
        await EnsureMovieExistsAsync(movieId);

        var review =
            await reviewRepository.FirstOrDefaultAsync(
                x =>
                    x.MovieId == movieId &&
                    x.UserId == userId);

        if (review is null)
            throw new NotFoundException(
                "You have not reviewed this movie yet.");

        var user =
            await identityService.FindByIdAsync(
                userId);

        return CreateReturnDto(
            review,
            user);
    }

    public async Task CreateAsync(
        Guid movieId,
        Guid userId,
        ReviewCreateDto dto)
    {
        await EnsureMovieExistsAsync(movieId);

        var alreadyExists =
            await reviewRepository.AnyAsync(
                x =>
                    x.MovieId == movieId &&
                    x.UserId == userId);

        if (alreadyExists)
            throw new AlreadyExistsException(
                "You have already rated or reviewed this movie.");

        var review =
            mapper.Map<MovieReview>(dto);

        review.MovieId = movieId;
        review.UserId = userId;

        await reviewRepository.AddAsync(review);
        await reviewRepository.SaveChangesAsync();
    }

    public async Task UpdateAsync(
        Guid movieId,
        Guid userId,
        ReviewUpdateDto dto)
    {
        await EnsureMovieExistsAsync(movieId);

        var review =
            await reviewRepository.FirstOrDefaultAsync(
                x =>
                    x.MovieId == movieId &&
                    x.UserId == userId,
                true);

        if (review is null)
            throw new NotFoundException(
                "You have not reviewed this movie yet.");

        mapper.Map(dto, review);

        review.UpdatedAt = DateTime.UtcNow;

        reviewRepository.Update(review);

        await reviewRepository.SaveChangesAsync();
    }

    public async Task DeleteMineAsync(
        Guid movieId,
        Guid userId)
    {
        await EnsureMovieExistsAsync(movieId);

        var review =
            await reviewRepository.FirstOrDefaultAsync(
                x =>
                    x.MovieId == movieId &&
                    x.UserId == userId,
                true);

        if (review is null)
            throw new NotFoundException(
                "You have not reviewed this movie yet.");

        reviewRepository.Delete(review);

        await reviewRepository.SaveChangesAsync();
    }

    public async Task DeleteByAdminAsync(
        Guid movieId,
        Guid reviewId)
    {
        await EnsureMovieExistsAsync(movieId);

        var review =
            await reviewRepository.FirstOrDefaultAsync(
                x =>
                    x.Id == reviewId &&
                    x.MovieId == movieId,
                true);

        if (review is null)
            throw new NotFoundException(
                "Review was not found.");

        reviewRepository.Delete(review);

        await reviewRepository.SaveChangesAsync();
    }

    private async Task EnsureMovieExistsAsync(
        Guid movieId)
    {
        var exists =
            await movieRepository.AnyAsync(
                x => x.Id == movieId);

        if (!exists)
            throw new NotFoundException(
                "Movie was not found.");
    }

    private static ReviewReturnDto CreateReturnDto(
        MovieReview review,
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