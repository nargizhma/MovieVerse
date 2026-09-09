using AutoMapper;
using MovieVerse.Abstractions.Media;
using MovieVerse.Dtos.Seasons;
using MovieVerse.Exceptions;
using MovieVerse.Models;
using MovieVerse.Repositories.Interfaces;
using MovieVerse.Services.Interfaces;

namespace MovieVerse.Services;

public class SeasonService(
    IGenericRepository<Season> seasonRepository,
    IGenericRepository<TVShow> tvShowRepository,
    IMapper mapper,
    IFileStorage fileStorage)
    : ISeasonService
{
    public async Task<List<SeasonReturnDto>>
        GetByTVShowIdAsync(
            Guid tvShowId)
    {
        await EnsureTVShowExistsAsync(
            tvShowId);

        var seasons =
            await seasonRepository.FindAllAsync(
                x =>
                    x.TVShowId == tvShowId,
                false,
                "Episodes");

        seasons =
            seasons
                .OrderBy(x =>
                    x.SeasonNumber)
                .ToList();

        return mapper.Map<
            List<SeasonReturnDto>>(
                seasons);
    }

    public async Task<SeasonReturnDto>
        GetByIdAsync(
            Guid id)
    {
        var season =
            await seasonRepository
                .FirstOrDefaultAsync(
                    x => x.Id == id,
                    false,
                    "Episodes");

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
            await seasonRepository.AnyAsync(
                x =>
                    x.TVShowId ==
                    tvShowId &&
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
            await seasonRepository.AnyAsync(
                x =>
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
            await seasonRepository
                .FirstOrDefaultAsync(
                    x => x.Id == id,
                    true,
                    "Episodes");

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
            fileStorage.Delete(
                image,
                "episodes");
        }
    }

    private async Task
        EnsureTVShowExistsAsync(
            Guid tvShowId)
    {
        var exists =
            await tvShowRepository.AnyAsync(
                x => x.Id == tvShowId);

        if (!exists)
            throw new NotFoundException(
                "TV show was not found.");
    }
}