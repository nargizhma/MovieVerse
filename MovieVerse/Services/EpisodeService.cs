using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MovieVerse.Dtos.Episodes;
using MovieVerse.Exceptions;
using MovieVerse.Extensions;
using MovieVerse.Models;
using MovieVerse.Models.Common;
using MovieVerse.Repositories.Interfaces;
using MovieVerse.Services.Interfaces;

namespace MovieVerse.Services;

public class EpisodeService(
    IGenericRepository<Episode> episodeRepository,
    IGenericRepository<Season> seasonRepository,
    IGenericRepository<Actor> actorRepository,
    IGenericRepository<Director> directorRepository,
    IGenericRepository<Writer> writerRepository,
    IMapper mapper,
    IWebHostEnvironment environment)
    : IEpisodeService
{
    public async Task<List<EpisodeReturnDto>>
        GetBySeasonIdAsync(
            Guid seasonId)
    {
        await EnsureSeasonExistsAsync(
            seasonId);

        var episodes =
            await episodeRepository.Query()
                .Where(x =>
                    x.SeasonId == seasonId)
                .Include(x =>
                    x.Reviews)
                .OrderBy(x =>
                    x.EpisodeNumber)
                .AsNoTracking()
                .ToListAsync();

        return mapper.Map<
            List<EpisodeReturnDto>>(
                episodes);
    }

    public async Task<EpisodeDetailsDto>
        GetByIdAsync(
            Guid id)
    {
        var episode =
            await episodeRepository.Query()

                .Include(x => x.Season)
                    .ThenInclude(x =>
                        x.TVShow)

                .Include(x =>
                    x.EpisodeActors)
                    .ThenInclude(x =>
                        x.Actor)

                .Include(x =>
                    x.EpisodeDirectors)
                    .ThenInclude(x =>
                        x.Director)

                .Include(x =>
                    x.EpisodeWriters)
                    .ThenInclude(x =>
                        x.Writer)

                .Include(x =>
                    x.Reviews)

                .AsNoTracking()
                .FirstOrDefaultAsync(
                    x => x.Id == id);

        if (episode is null)
            throw new NotFoundException(
                "Episode was not found.");

        return mapper.Map<
            EpisodeDetailsDto>(
                episode);
    }

    public async Task CreateAsync(
        Guid seasonId,
        EpisodeCreateDto dto)
    {
        await EnsureSeasonExistsAsync(
            seasonId);

        var exists =
            await episodeRepository.Query()
                .AnyAsync(x =>
                    x.SeasonId == seasonId &&
                    x.EpisodeNumber ==
                    dto.EpisodeNumber);

        if (exists)
            throw new AlreadyExistsException(
                "This episode number already exists in the season.");

        await ValidateRelatedEntitiesAsync(
            dto.Actors,
            dto.DirectorIds,
            dto.WriterIds);

        var episode =
            mapper.Map<Episode>(dto);

        episode.SeasonId =
            seasonId;

        SetRelationships(
            episode,
            dto.Actors,
            dto.DirectorIds,
            dto.WriterIds);

        var folderPath =
            environment.GetImageFolderPath(
                "episodes");

        string? newImage = null;

        try
        {
            if (dto.Image is not null)
            {
                newImage =
                    await dto.Image
                        .SaveFileAsync(
                            folderPath);

                episode.ImageUrl =
                    newImage;
            }

            await episodeRepository.AddAsync(
                episode);

            await episodeRepository
                .SaveChangesAsync();
        }
        catch
        {
            FileManager.DeleteFile(
                newImage,
                folderPath);

            throw;
        }
    }

    public async Task UpdateAsync(
        Guid id,
        EpisodeUpdateDto dto)
    {
        var episode =
            await episodeRepository.Query()
                .Include(x => x.EpisodeActors)
                .Include(x => x.EpisodeDirectors)
                .Include(x => x.EpisodeWriters)
                .FirstOrDefaultAsync(
                    x => x.Id == id);

        if (episode is null)
            throw new NotFoundException(
                "Episode was not found.");

        var duplicateNumber =
            await episodeRepository.Query()
                .AnyAsync(x =>
                    x.Id != id &&
                    x.SeasonId ==
                    episode.SeasonId &&
                    x.EpisodeNumber ==
                    dto.EpisodeNumber);

        if (duplicateNumber)
            throw new AlreadyExistsException(
                "This episode number already exists in the season.");

        await ValidateRelatedEntitiesAsync(
            dto.Actors,
            dto.DirectorIds,
            dto.WriterIds);

        var oldImage =
            episode.ImageUrl;

        mapper.Map(
            dto,
            episode);

        SetRelationships(
            episode,
            dto.Actors,
            dto.DirectorIds,
            dto.WriterIds);

        var folderPath =
            environment.GetImageFolderPath(
                "episodes");

        string? newImage = null;

        if (dto.Image is not null)
        {
            newImage =
                await dto.Image
                    .SaveFileAsync(
                        folderPath);

            episode.ImageUrl =
                newImage;
        }

        try
        {
            episodeRepository.Update(
                episode);

            await episodeRepository
                .SaveChangesAsync();
        }
        catch
        {
            FileManager.DeleteFile(
                newImage,
                folderPath);

            throw;
        }

        if (newImage is not null)
        {
            FileManager.DeleteFile(
                oldImage,
                folderPath);
        }
    }

    public async Task DeleteAsync(
        Guid id)
    {
        var episode =
            await episodeRepository
                .GetByIdAsync(id);

        if (episode is null)
            throw new NotFoundException(
                "Episode was not found.");

        var image =
            episode.ImageUrl;

        episodeRepository.Delete(
            episode);

        await episodeRepository
            .SaveChangesAsync();

        FileManager.DeleteFile(
            image,
            environment.GetImageFolderPath("episodes"));
    }

    private static void SetRelationships(
        Episode episode,
        IEnumerable<EpisodeActorInputDto> actors,
        IEnumerable<Guid> directorIds,
        IEnumerable<Guid> writerIds)
    {
        episode.EpisodeActors.Clear();

        episode.EpisodeActors.AddRange(
            actors.Select(actor =>
                new EpisodeActor
                {
                    ActorId =
                        actor.ActorId,

                    CharacterName =
                        actor.CharacterName,

                    CastOrder =
                        actor.CastOrder
                }));


        episode.EpisodeDirectors.Clear();

        episode.EpisodeDirectors.AddRange(
            directorIds
                .Distinct()
                .Select(directorId =>
                    new EpisodeDirector
                    {
                        DirectorId =
                            directorId
                    }));


        episode.EpisodeWriters.Clear();

        episode.EpisodeWriters.AddRange(
            writerIds
                .Distinct()
                .Select(writerId =>
                    new EpisodeWriter
                    {
                        WriterId =
                            writerId
                    }));
    }

    private async Task
        ValidateRelatedEntitiesAsync(
            IEnumerable<EpisodeActorInputDto> actors,
            IEnumerable<Guid> directorIds,
            IEnumerable<Guid> writerIds)
    {
        await EnsureIdsExistAsync(
            actorRepository.Query(),
            actors.Select(
                x => x.ActorId),
            "Actor");

        await EnsureIdsExistAsync(
            directorRepository.Query(),
            directorIds,
            "Director");

        await EnsureIdsExistAsync(
            writerRepository.Query(),
            writerIds,
            "Writer");
    }

    private static async Task EnsureIdsExistAsync<T>(
        IQueryable<T> query,
        IEnumerable<Guid> ids,
        string entityName)
        where T : BaseEntity
    {
        var requestedIds =
            ids.Distinct().ToList();

        if (requestedIds.Count == 0)
            return;

        var existingIds =
            await query
                .Where(x =>
                    requestedIds.Contains(
                        x.Id))
                .Select(x =>
                    x.Id)
                .ToListAsync();

        var missingId =
            requestedIds
                .Except(existingIds)
                .FirstOrDefault();

        if (missingId != Guid.Empty)
        {
            throw new BadRequestException(
                $"{entityName} with id '{missingId}' was not found.");
        }
    }

    private async Task
        EnsureSeasonExistsAsync(
            Guid seasonId)
    {
        var exists =
            await seasonRepository.Query()
                .AnyAsync(
                    x => x.Id == seasonId);

        if (!exists)
            throw new NotFoundException(
                "Season was not found.");
    }

}