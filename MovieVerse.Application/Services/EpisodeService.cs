using AutoMapper;
using MovieVerse.Abstractions.Media;
using MovieVerse.Dtos.Episodes;
using MovieVerse.Exceptions;
using MovieVerse.Models;
using MovieVerse.Models.Common;
using MovieVerse.Repositories.Interfaces;
using MovieVerse.Requests.Episodes;
using MovieVerse.Services.Interfaces;

namespace MovieVerse.Services;

public class EpisodeService(
    IGenericRepository<Episode> episodeRepository,
    IGenericRepository<Season> seasonRepository,
    IGenericRepository<Actor> actorRepository,
    IGenericRepository<Director> directorRepository,
    IGenericRepository<Writer> writerRepository,
    IMapper mapper,
    IFileStorage fileStorage)
    : IEpisodeService
{
    public async Task<List<EpisodeReturnDto>>
        GetBySeasonIdAsync(
            Guid seasonId)
    {
        await EnsureSeasonExistsAsync(
            seasonId);

        var episodes =
            await episodeRepository.FindAllAsync(
                x => x.SeasonId == seasonId,
                false,
                "Reviews");

        episodes =
            episodes
                .OrderBy(x => x.EpisodeNumber)
                .ToList();

        return mapper.Map<
            List<EpisodeReturnDto>>(
                episodes);
    }

    public async Task<EpisodeDetailsDto>
        GetByIdAsync(
            Guid id)
    {
        var episode =
            await episodeRepository
                .FirstOrDefaultAsync(
                    x => x.Id == id,
                    false,
                    "Season.TVShow",
                    "EpisodeActors.Actor",
                    "EpisodeDirectors.Director",
                    "EpisodeWriters.Writer",
                    "Reviews");

        if (episode is null)
            throw new NotFoundException(
                "Episode was not found.");

        return mapper.Map<EpisodeDetailsDto>(
            episode);
    }

    public async Task CreateAsync(
        Guid seasonId,
        EpisodeCreateRequest request)
    {
        await EnsureSeasonExistsAsync(
            seasonId);

        var exists =
            await episodeRepository.AnyAsync(
                x =>
                    x.SeasonId == seasonId &&
                    x.EpisodeNumber ==
                    request.EpisodeNumber);

        if (exists)
            throw new AlreadyExistsException(
                "This episode number already exists in the season.");

        await ValidateRelatedEntitiesAsync(
            request.Actors,
            request.DirectorIds,
            request.WriterIds);

        var episode =
            mapper.Map<Episode>(
                request);

        episode.SeasonId =
            seasonId;

        SetRelationships(
            episode,
            request.Actors,
            request.DirectorIds,
            request.WriterIds);

        string? newImage = null;

        try
        {
            if (request.Image is not null)
            {
                newImage =
                    await fileStorage.SaveAsync(
                        request.Image,
                        "episodes");

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
            fileStorage.Delete(
                newImage,
                "episodes");

            throw;
        }
    }

    public async Task UpdateAsync(
        Guid id,
        EpisodeUpdateRequest request)
    {
        var episode =
            await episodeRepository
                .FirstOrDefaultAsync(
                    x => x.Id == id,
                    true,
                    "EpisodeActors",
                    "EpisodeDirectors",
                    "EpisodeWriters");

        if (episode is null)
            throw new NotFoundException(
                "Episode was not found.");

        var duplicateNumber =
            await episodeRepository.AnyAsync(
                x =>
                    x.Id != id &&
                    x.SeasonId ==
                    episode.SeasonId &&
                    x.EpisodeNumber ==
                    request.EpisodeNumber);

        if (duplicateNumber)
            throw new AlreadyExistsException(
                "This episode number already exists in the season.");

        await ValidateRelatedEntitiesAsync(
            request.Actors,
            request.DirectorIds,
            request.WriterIds);

        var oldImage =
            episode.ImageUrl;

        mapper.Map(
            request,
            episode);

        SetRelationships(
            episode,
            request.Actors,
            request.DirectorIds,
            request.WriterIds);

        string? newImage = null;

        if (request.Image is not null)
        {
            newImage =
                await fileStorage.SaveAsync(
                    request.Image,
                    "episodes");

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
            fileStorage.Delete(
                newImage,
                "episodes");

            throw;
        }

        if (newImage is not null)
        {
            fileStorage.Delete(
                oldImage,
                "episodes");
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

        fileStorage.Delete(
            image,
            "episodes");
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
            actorRepository,
            actors.Select(x =>
                x.ActorId),
            "Actor");

        await EnsureIdsExistAsync(
            directorRepository,
            directorIds,
            "Director");

        await EnsureIdsExistAsync(
            writerRepository,
            writerIds,
            "Writer");
    }

    private static async Task
        EnsureIdsExistAsync<T>(
            IGenericRepository<T> repository,
            IEnumerable<Guid> ids,
            string entityName)
        where T : BaseEntity
    {
        var requestedIds =
            ids
                .Distinct()
                .ToList();

        if (requestedIds.Count == 0)
            return;

        var existing =
            await repository.FindAllAsync(
                x =>
                    requestedIds.Contains(
                        x.Id));

        var existingIds =
            existing
                .Select(x =>
                    x.Id)
                .ToHashSet();

        var missingId =
            requestedIds
                .FirstOrDefault(x =>
                    !existingIds.Contains(x));

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
            await seasonRepository.AnyAsync(
                x => x.Id == seasonId);

        if (!exists)
            throw new NotFoundException(
                "Season was not found.");
    }
}