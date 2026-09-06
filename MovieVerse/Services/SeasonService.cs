using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MovieVerse.Dtos.Seasons;
using MovieVerse.Exceptions;
using MovieVerse.Extensions;
using MovieVerse.Models;
using MovieVerse.Repositories.Interfaces;
using MovieVerse.Services.Interfaces;

namespace MovieVerse.Services;

public class SeasonService(
    IGenericRepository<Season> seasonRepository,
    IGenericRepository<TVShow> tvShowRepository,
    IMapper mapper,
    IWebHostEnvironment environment)
    : ISeasonService
{
    public async Task<List<SeasonReturnDto>>
        GetByTVShowIdAsync(
            Guid tvShowId)
    {
        await EnsureTVShowExistsAsync(
            tvShowId);

        var seasons =
            await seasonRepository.Query()
                .Where(x =>
                    x.TVShowId == tvShowId)
                .Include(x =>
                    x.Episodes)
                .OrderBy(x =>
                    x.SeasonNumber)
                .AsNoTracking()
                .ToListAsync();

        return mapper.Map<
            List<SeasonReturnDto>>(
                seasons);
    }

    public async Task<SeasonReturnDto>
        GetByIdAsync(
            Guid id)
    {
        var season =
            await seasonRepository.Query()
                .Include(x =>
                    x.Episodes)
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    x => x.Id == id);

        if (season is null)
            throw new NotFoundException(
                "Season was not found.");

        return mapper.Map<
            SeasonReturnDto>(
                season);
    }

    public async Task CreateAsync(
        Guid tvShowId,
        SeasonCreateDto dto)
    {
        await EnsureTVShowExistsAsync(
            tvShowId);

        var exists =
            await seasonRepository.Query()
                .AnyAsync(x =>
                    x.TVShowId == tvShowId &&
                    x.SeasonNumber ==
                    dto.SeasonNumber);

        if (exists)
            throw new AlreadyExistsException(
                "This season number already exists for the TV show.");

        var season =
            mapper.Map<Season>(dto);

        season.TVShowId =
            tvShowId;

        await seasonRepository.AddAsync(
            season);

        await seasonRepository
            .SaveChangesAsync();
    }

    public async Task UpdateAsync(
        Guid id,
        SeasonUpdateDto dto)
    {
        var season =
            await seasonRepository
                .GetByIdAsync(id);

        if (season is null)
            throw new NotFoundException(
                "Season was not found.");

        var exists =
            await seasonRepository.Query()
                .AnyAsync(x =>
                    x.Id != id &&
                    x.TVShowId ==
                    season.TVShowId &&
                    x.SeasonNumber ==
                    dto.SeasonNumber);

        if (exists)
            throw new AlreadyExistsException(
                "This season number already exists for the TV show.");

        mapper.Map(
            dto,
            season);

        seasonRepository.Update(
            season);

        await seasonRepository
            .SaveChangesAsync();
    }

    public async Task DeleteAsync(
        Guid id)
    {
        var season =
            await seasonRepository.Query()
                .Include(x => x.Episodes)
                .FirstOrDefaultAsync(x =>
                    x.Id == id);

        if (season is null)
            throw new NotFoundException(
                "Season was not found.");

        var episodeImages =
            season.Episodes
                .Select(x => x.ImageUrl)
                .Where(x =>
                    !string.IsNullOrWhiteSpace(x))
                .ToList();

        seasonRepository.Delete(
            season);

        await seasonRepository
            .SaveChangesAsync();

        foreach (var image in episodeImages)
        {
            FileManager.DeleteFile(
                image,
                environment.GetImageFolderPath("episodes"));
        }
    }

    private async Task
        EnsureTVShowExistsAsync(
            Guid tvShowId)
    {
        var exists =
            await tvShowRepository.Query()
                .AnyAsync(
                    x => x.Id == tvShowId);

        if (!exists)
            throw new NotFoundException(
                "TV show was not found.");
    }
}