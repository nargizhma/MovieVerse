using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MovieVerse.Dtos.Common;
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
    public async Task<PagedResultDto<TVShowReturnDto>> GetAllAsync(
        CatalogFilterDto filter)
    {
        var query =
            tvShowRepository.Query()
                .Include(x => x.TVShowGenres)
                    .ThenInclude(x => x.Genre)
                .Include(x => x.Reviews)
                .AsNoTracking();

        if (!string.IsNullOrWhiteSpace(
                filter.Search))
        {
            var search =
                filter.Search.Trim();

            query = query.Where(x =>
                EF.Functions.Like(
                    x.Title,
                    $"%{search}%") ||
                (x.OriginalTitle != null &&
                 EF.Functions.Like(
                     x.OriginalTitle,
                     $"%{search}%")));
        }

        if (filter.GenreId.HasValue)
        {
            var genreId =
                filter.GenreId.Value;

            query = query.Where(x =>
                x.TVShowGenres.Any(g =>
                    g.GenreId == genreId));
        }

        if (filter.ReleaseYear.HasValue)
        {
            var releaseYear =
                filter.ReleaseYear.Value;

            query = query.Where(x =>
                x.ReleaseDate.Year ==
                releaseYear);
        }

        if (filter.ActorId.HasValue)
        {
            var actorId =
                filter.ActorId.Value;

            query = query.Where(x =>
                x.TVShowActors.Any(a =>
                    a.ActorId == actorId)
                ||
                x.Seasons.Any(season =>
                    season.Episodes.Any(episode =>
                        episode.EpisodeActors.Any(a =>
                            a.ActorId == actorId))));
        }

        if (filter.DirectorId.HasValue)
        {
            var directorId =
                filter.DirectorId.Value;

            query = query.Where(x =>
                x.Seasons.Any(season =>
                    season.Episodes.Any(
                        episode =>
                            episode.EpisodeDirectors
                                .Any(d =>
                                    d.DirectorId ==
                                    directorId))));
        }

        if (filter.MinRating.HasValue)
        {
            var minRating =
                filter.MinRating.Value;

            query = query.Where(x =>
                x.Reviews.Any() &&
                x.Reviews.Average(r =>
                    r.Rating) >= minRating);
        }

        if (filter.MaxRating.HasValue)
        {
            var maxRating =
                filter.MaxRating.Value;

            query = query.Where(x =>
                x.Reviews.Any() &&
                x.Reviews.Average(r =>
                    r.Rating) <= maxRating);
        }

        query =
            ApplySorting(
                query,
                filter);

        var totalCount =
            await query.CountAsync();

        var tvShows =
            await query
                .Skip(
                    (filter.PageNumber - 1) *
                    filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

        var items =
            mapper.Map<List<TVShowReturnDto>>(
                tvShows);

        return new PagedResultDto<TVShowReturnDto>
        {
            Items = items,
            PageNumber =
                filter.PageNumber,
            PageSize =
                filter.PageSize,
            TotalCount =
                totalCount,
            TotalPages =
                (int)Math.Ceiling(
                    totalCount /
                    (double)filter.PageSize)
        };
    }

    private static IQueryable<TVShow> ApplySorting(
        IQueryable<TVShow> query,
        CatalogFilterDto filter)
    {
        var sortBy =
            filter.SortBy?
                .Trim()
                .ToLowerInvariant();

        return sortBy switch
        {
            "title" =>
                filter.SortDescending
                    ? query.OrderByDescending(
                        x => x.Title)
                    : query.OrderBy(
                        x => x.Title),

            "rating" =>
                filter.SortDescending
                    ? query.OrderByDescending(
                        x => x.Reviews.Any()
                            ? x.Reviews.Average(
                                r => r.Rating)
                            : 0m)
                    : query.OrderBy(
                        x => x.Reviews.Any()
                            ? x.Reviews.Average(
                                r => r.Rating)
                            : 0m),

            "year" =>
                filter.SortDescending
                    ? query.OrderByDescending(
                        x => x.ReleaseDate)
                    : query.OrderBy(
                        x => x.ReleaseDate),

            _ =>
                query.OrderByDescending(
                    x => x.ReleaseDate)
        };
    }

    public async Task<TVShowDetailsDto> GetByIdAsync(Guid id)
    {
        var tvShow = await tvShowRepository.Query()
            .Include(x => x.TVShowDetail)
            .Include(x => x.TVShowGenres)
                .ThenInclude(x => x.Genre)
            .Include(x => x.TVShowActors)
                .ThenInclude(x => x.Actor)
            .Include(x => x.Seasons)
                .ThenInclude(x => x.Episodes)
            .Include(x => x.Reviews)
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);

        if (tvShow is null)
            throw new NotFoundException("TV show was not found.");

        return mapper.Map<TVShowDetailsDto>(tvShow);
    }

    public async Task CreateAsync(TVShowCreateDto dto)
    {
        await ValidateRelatedEntitiesAsync(dto.GenreIds, dto.Actors);

        var tvShow = mapper.Map<TVShow>(dto);

        tvShow.TVShowDetail = mapper.Map<TVShowDetail>(dto);

        SetRelationships(tvShow, dto.GenreIds, dto.Actors);

        if (dto.PosterImage is not null)
        {
            tvShow.PosterUrl = await dto.PosterImage.SaveFileAsync(
                GetImageFolderPath());
        }

        await tvShowRepository.AddAsync(tvShow);
        await tvShowRepository.SaveChangesAsync();
    }

    public async Task UpdateAsync(Guid id, TVShowUpdateDto dto)
    {
        var tvShow = await tvShowRepository.Query()
            .Include(x => x.TVShowDetail)
            .Include(x => x.TVShowGenres)
            .Include(x => x.TVShowActors)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (tvShow is null)
            throw new NotFoundException("TV show was not found.");

        await ValidateRelatedEntitiesAsync(dto.GenreIds, dto.Actors);

        var oldPoster = tvShow.PosterUrl;

        mapper.Map(dto, tvShow);

        if (tvShow.TVShowDetail is null)
        {
            tvShow.TVShowDetail = mapper.Map<TVShowDetail>(dto);
        }
        else
        {
            mapper.Map(dto, tvShow.TVShowDetail);
        }

        SetRelationships(tvShow, dto.GenreIds, dto.Actors);

        if (dto.PosterImage is not null)
        {
            tvShow.PosterUrl = await dto.PosterImage.SaveFileAsync(
                GetImageFolderPath());
        }

        tvShowRepository.Update(tvShow);
        await tvShowRepository.SaveChangesAsync();

        if (dto.PosterImage is not null)
        {
            FileManager.DeleteFile(oldPoster, GetImageFolderPath());
        }
    }

    public async Task DeleteAsync(Guid id)
    {
        var tvShow =
            await tvShowRepository.Query()
                .Include(x => x.Seasons)
                    .ThenInclude(x =>
                        x.Episodes)
                .FirstOrDefaultAsync(x =>
                    x.Id == id);

        if (tvShow is null)
            throw new NotFoundException(
                "TV show was not found.");

        var poster =
            tvShow.PosterUrl;

        var episodeImages =
            tvShow.Seasons
                .SelectMany(x =>
                    x.Episodes)
                .Select(x =>
                    x.ImageUrl)
                .Where(x =>
                    !string.IsNullOrWhiteSpace(x))
                .ToList();

        tvShowRepository.Delete(
            tvShow);

        await tvShowRepository
            .SaveChangesAsync();

        FileManager.DeleteFile(
            poster,
            GetImageFolderPath());

        foreach (var image in episodeImages)
        {
            FileManager.DeleteFile(
                image,
                GetEpisodeImageFolderPath());
        }
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
                .Select(genreId => new TVShowGenre
                {
                    GenreId = genreId
                }));

        tvShow.TVShowActors.Clear();

        tvShow.TVShowActors.AddRange(
            actors.Select(actor => new TVShowActor
            {
                ActorId = actor.ActorId,
                CharacterName = actor.CharacterName,
                CastOrder = actor.CastOrder
            }));
    }

    private async Task ValidateRelatedEntitiesAsync(
        IEnumerable<Guid> genreIds,
        IEnumerable<TVShowActorInputDto> actors)
    {
        await EnsureIdsExistAsync(
            genreRepository.Query(),
            genreIds,
            "Genre");

        await EnsureIdsExistAsync(
            actorRepository.Query(),
            actors.Select(x => x.ActorId),
            "Actor");
    }

    private static async Task EnsureIdsExistAsync<T>(
        IQueryable<T> query,
        IEnumerable<Guid> ids,
        string entityName)
        where T : BaseEntity
    {
        var requestedIds = ids.Distinct().ToList();

        if (requestedIds.Count == 0)
            return;

        var existingIds = await query
            .Where(x => requestedIds.Contains(x.Id))
            .Select(x => x.Id)
            .ToListAsync();

        var missingId = requestedIds
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
        var webRootPath = environment.WebRootPath
            ?? Path.Combine(environment.ContentRootPath, "wwwroot");

        return Path.Combine(webRootPath, "images", "tvshows");
    }
    private string GetEpisodeImageFolderPath()
    {
        var webRootPath =
            environment.WebRootPath
            ?? Path.Combine(
                environment.ContentRootPath,
                "wwwroot");

        return Path.Combine(
            webRootPath,
            "images",
            "episodes");
    }
}
