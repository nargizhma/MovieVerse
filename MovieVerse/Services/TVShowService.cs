using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MovieVerse.Dtos.TVShows;
using MovieVerse.Exceptions;
using MovieVerse.Extensions;
using MovieVerse.Models;
using MovieVerse.Models.Common;
using MovieVerse.Repositories.Interfaces;
using MovieVerse.Services.Interfaces;

namespace MovieVerse.Services;

public class TVShowService(
    IGenericRepository<TVShow> tvShowRepository,
    IGenericRepository<Genre> genreRepository,
    IGenericRepository<Actor> actorRepository,
    IMapper mapper,
    IWebHostEnvironment environment)
    : ITVShowService
{
    public async Task<List<TVShowReturnDto>> GetAllAsync()
    {
        var tvShows =
            await tvShowRepository.Query()
                .Include(x => x.TVShowGenres)
                    .ThenInclude(x => x.Genre)
                .Include(x => x.Reviews)
                .AsNoTracking()
                .ToListAsync();

        return mapper.Map<List<TVShowReturnDto>>(
            tvShows);
    }

    public async Task<TVShowDetailsDto> GetByIdAsync(
        Guid id)
    {
        var tvShow =
            await tvShowRepository.Query()

                .Include(x => x.TVShowDetail)

                .Include(x => x.TVShowGenres)
                    .ThenInclude(x => x.Genre)

                .Include(x => x.TVShowActors)
                    .ThenInclude(x => x.Actor)

                .Include(x => x.Seasons)
                    .ThenInclude(x => x.Episodes)

                .Include(x => x.Reviews)

                .AsNoTracking()
                .FirstOrDefaultAsync(
                    x => x.Id == id);

        if (tvShow is null)
            throw new NotFoundException(
                "TV show was not found.");

        return mapper.Map<TVShowDetailsDto>(
            tvShow);
    }

    public async Task CreateAsync(
        TVShowCreateDto dto)
    {
        await ValidateRelatedEntitiesAsync(
            dto.GenreIds,
            dto.Actors);

        var tvShow =
            mapper.Map<TVShow>(dto);

        tvShow.TVShowDetail =
            mapper.Map<TVShowDetail>(dto);

        SetRelationships(
            tvShow,
            dto.GenreIds,
            dto.Actors);

        if (dto.PosterImage is not null)
        {
            tvShow.PosterUrl =
                await dto.PosterImage.SaveFileAsync(
                    GetImageFolderPath());
        }

        await tvShowRepository.AddAsync(
            tvShow);

        await tvShowRepository
            .SaveChangesAsync();
    }

    public async Task UpdateAsync(
        Guid id,
        TVShowUpdateDto dto)
    {
        var tvShow =
            await tvShowRepository.Query()

                .Include(x =>
                    x.TVShowDetail)

                .Include(x =>
                    x.TVShowGenres)

                .Include(x =>
                    x.TVShowActors)

                .FirstOrDefaultAsync(
                    x => x.Id == id);

        if (tvShow is null)
            throw new NotFoundException(
                "TV show was not found.");

        await ValidateRelatedEntitiesAsync(
            dto.GenreIds,
            dto.Actors);

        var oldPoster =
            tvShow.PosterUrl;

        mapper.Map(
            dto,
            tvShow);

        if (tvShow.TVShowDetail is null)
        {
            tvShow.TVShowDetail =
                mapper.Map<TVShowDetail>(
                    dto);
        }
        else
        {
            mapper.Map(
                dto,
                tvShow.TVShowDetail);
        }

        SetRelationships(
            tvShow,
            dto.GenreIds,
            dto.Actors);

        if (dto.PosterImage is not null)
        {
            tvShow.PosterUrl =
                await dto.PosterImage
                    .SaveFileAsync(
                        GetImageFolderPath());
        }

        tvShowRepository.Update(
            tvShow);

        await tvShowRepository
            .SaveChangesAsync();

        if (dto.PosterImage is not null)
        {
            FileManager.DeleteFile(
                oldPoster,
                GetImageFolderPath());
        }
    }

    public async Task DeleteAsync(
        Guid id)
    {
        var tvShow =
            await tvShowRepository
                .GetByIdAsync(id);

        if (tvShow is null)
            throw new NotFoundException(
                "TV show was not found.");

        var poster =
            tvShow.PosterUrl;

        tvShowRepository.Delete(
            tvShow);

        await tvShowRepository
            .SaveChangesAsync();

        FileManager.DeleteFile(
            poster,
            GetImageFolderPath());
    }

    private static void SetRelationships(
        TVShow tvShow,
        IEnumerable<Guid> genreIds,
        IEnumerable<TVShowActorInputDto> actors)
    {
        tvShow.TVShowGenres.Clear();

        tvShow.TVShowGenres.AddRange(
            genreIds
                .Distinct()
                .Select(genreId =>
                    new TVShowGenre
                    {
                        GenreId =
                            genreId
                    }));


        tvShow.TVShowActors.Clear();

        tvShow.TVShowActors.AddRange(
            actors.Select(actor =>
                new TVShowActor
                {
                    ActorId =
                        actor.ActorId,

                    CharacterName =
                        actor.CharacterName,

                    CastOrder =
                        actor.CastOrder
                }));
    }

    private async Task
        ValidateRelatedEntitiesAsync(
            IEnumerable<Guid> genreIds,
            IEnumerable<TVShowActorInputDto> actors)
    {
        await EnsureIdsExistAsync(
            genreRepository.Query(),
            genreIds,
            "Genre");

        await EnsureIdsExistAsync(
            actorRepository.Query(),
            actors.Select(
                x => x.ActorId),
            "Actor");
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
                .Select(x => x.Id)
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

    private string GetImageFolderPath()
    {
        var webRootPath =
            environment.WebRootPath
            ?? Path.Combine(
                environment.ContentRootPath,
                "wwwroot");

        return Path.Combine(
            webRootPath,
            "images",
            "tvshows");
    }
}