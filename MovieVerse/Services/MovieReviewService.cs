using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MovieVerse.Dtos.Reviews;
using MovieVerse.Exceptions;
using MovieVerse.Models;
using MovieVerse.Repositories.Interfaces;
using MovieVerse.Services.Interfaces;

namespace MovieVerse.Services;

public class MovieReviewService(
    IGenericRepository<MovieReview> reviewRepository,
    IGenericRepository<Movie> movieRepository,
    IMapper mapper)
    : IMovieReviewService
{
    public async Task<List<ReviewReturnDto>> GetAllAsync(
        Guid movieId)
    {
        await EnsureMovieExistsAsync(movieId);

        var reviews =
            await reviewRepository.Query()
                .Where(x => x.MovieId == movieId)
                .Include(x => x.User)
                    .ThenInclude(x => x.Profile)
                .OrderByDescending(x => x.CreatedAt)
                .AsNoTracking()
                .ToListAsync();

        return mapper.Map<List<ReviewReturnDto>>(
            reviews);
    }

    public async Task<ReviewReturnDto> GetMineAsync(
        Guid movieId,
        Guid userId)
    {
        await EnsureMovieExistsAsync(movieId);

        var review =
            await reviewRepository.Query()
                .Include(x => x.User)
                    .ThenInclude(x => x.Profile)
                .AsNoTracking()
                .FirstOrDefaultAsync(x =>
                    x.MovieId == movieId &&
                    x.UserId == userId);

        if (review is null)
            throw new NotFoundException(
                "You have not reviewed this movie yet.");

        return mapper.Map<ReviewReturnDto>(
            review);
    }

    public async Task CreateAsync(
        Guid movieId,
        Guid userId,
        ReviewCreateDto dto)
    {
        await EnsureMovieExistsAsync(movieId);

        var alreadyExists =
            await reviewRepository.Query()
                .AnyAsync(x =>
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
            await reviewRepository.Query()
                .FirstOrDefaultAsync(x =>
                    x.MovieId == movieId &&
                    x.UserId == userId);

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
            await reviewRepository.Query()
                .FirstOrDefaultAsync(x =>
                    x.MovieId == movieId &&
                    x.UserId == userId);

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
            await reviewRepository.Query()
                .FirstOrDefaultAsync(x =>
                    x.Id == reviewId &&
                    x.MovieId == movieId);

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
            await movieRepository.Query()
                .AnyAsync(x => x.Id == movieId);

        if (!exists)
            throw new NotFoundException(
                "Movie was not found.");
    }
}
