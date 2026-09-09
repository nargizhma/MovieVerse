using AutoMapper;
using MovieVerse.Abstractions.Media;
using MovieVerse.Dtos.Common;
using MovieVerse.Dtos.TVShows;
using MovieVerse.Exceptions;
using MovieVerse.Models;
using MovieVerse.Models.Common;
using MovieVerse.Repositories.Interfaces;
using MovieVerse.Requests.TVShows;
using MovieVerse.Services.Interfaces;

namespace MovieVerse.Services;

public class TVShowService(
    IGenericRepository<TVShow> tvShowRepository,
    IGenericRepository<Genre> genreRepository,
    IGenericRepository<Actor> actorRepository,
    IMapper mapper,
    IFileStorage fileStorage)
    : ITVShowService
{
    public async Task<PagedResultDto<TVShowReturnDto>>
        GetAllAsync(
            CatalogFilterDto filter)
    {
        var search =
            filter.Search?
                .Trim()
                .ToLowerInvariant();

        var genreId =
            filter.GenreId;

        var releaseYear =
            filter.ReleaseYear;

        var actorId =
            filter.ActorId;

        var directorId =
            filter.DirectorId;

        var minRating =
            filter.MinRating;

        var maxRating =
            filter.MaxRating;

        var tvShows =
            await tvShowRepository.FindAllAsync(
                x =>
                    (search == null ||
                     x.Title.ToLower()
                         .Contains(search) ||
                     (x.OriginalTitle != null &&
                      x.OriginalTitle.ToLower()
                          .Contains(search)))
                    &&
                    (!genreId.HasValue ||
                     x.TVShowGenres.Any(g =>
                         g.GenreId ==
                         genreId.Value))
                    &&
                    (!releaseYear.HasValue ||
                     x.ReleaseDate.Year ==
                     releaseYear.Value)
                    &&
                    (!actorId.HasValue ||
                     x.TVShowActors.Any(a =>
                         a.ActorId ==
                         actorId.Value) ||
                     x.Seasons.Any(season =>
                         season.Episodes.Any(
                             episode =>
                                 episode
                                     .EpisodeActors
                                     .Any(a =>
                                         a.ActorId ==
                                         actorId.Value))))
                    &&
                    (!directorId.HasValue ||
                     x.Seasons.Any(season =>
                         season.Episodes.Any(
                             episode =>
                                 episode
                                     .EpisodeDirectors
                                     .Any(d =>
                                         d.DirectorId ==
                                         directorId.Value))))
                    &&
                    (!minRating.HasValue ||
                     (x.Reviews.Any() &&
                      x.Reviews.Average(r =>
                          r.Rating) >=
                      minRating.Value))
                    &&
                    (!maxRating.HasValue ||
                     (x.Reviews.Any() &&
                      x.Reviews.Average(r =>
                          r.Rating) <=
                      maxRating.Value)),
                false,
                "TVShowGenres.Genre",
                "Reviews");

        IEnumerable<TVShow> sorted =
            ApplySorting(
                tvShows,
                filter);

        var totalCount =
            tvShows.Count;

        var pageItems =
            sorted
                .Skip(
                    (filter.PageNumber - 1) *
                    filter.PageSize)
                .Take(filter.PageSize)
                .ToList();

        return new PagedResultDto<TVShowReturnDto>
        {
            Items =
                mapper.Map<List<TVShowReturnDto>>(
                    pageItems),

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

    private static IEnumerable<TVShow>
        ApplySorting(
            IEnumerable<TVShow> tvShows,
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
                    ? tvShows.OrderByDescending(
                        x => x.Title)
                    : tvShows.OrderBy(
                        x => x.Title),

            "rating" =>
                filter.SortDescending
                    ? tvShows.OrderByDescending(
                        GetAverageRating)
                    : tvShows.OrderBy(
                        GetAverageRating),

            "year" =>
                filter.SortDescending
                    ? tvShows.OrderByDescending(
                        x => x.ReleaseDate)
                    : tvShows.OrderBy(
                        x => x.ReleaseDate),

            _ =>
                tvShows.OrderByDescending(
                    x => x.ReleaseDate)
        };
    }

    private static decimal GetAverageRating(
        TVShow tvShow)
    {
        return tvShow.Reviews.Count == 0
            ? 0m
            : tvShow.Reviews.Average(
                x => x.Rating);
    }

    public async Task<TVShowDetailsDto>
        GetByIdAsync(
            Guid id)
    {
        var tvShow =
            await tvShowRepository
                .FirstOrDefaultAsync(
                    x => x.Id == id,
                    false,
                    "TVShowDetail",
                    "TVShowGenres.Genre",
                    "TVShowActors.Actor",
                    "Seasons.Episodes",
                    "Reviews");

        if (tvShow is null)
            throw new NotFoundException(
                "TV show was not found.");

        return mapper.Map<TVShowDetailsDto>(
            tvShow);
    }

    public async Task CreateAsync(
        TVShowCreateRequest request)
    {
        await ValidateRelatedEntitiesAsync(
            request.GenreIds,
            request.Actors);

        var tvShow =
            mapper.Map<TVShow>(
                request);

        tvShow.TVShowDetail =
            mapper.Map<TVShowDetail>(
                request);

        SetRelationships(
            tvShow,
            request.GenreIds,
            request.Actors);

        string? newPoster = null;

        try
        {
            if (request.PosterImage is not null)
            {
                newPoster =
                    await fileStorage.SaveAsync(
                        request.PosterImage,
                        "tvshows");

                tvShow.PosterUrl =
                    newPoster;
            }

            await tvShowRepository.AddAsync(
                tvShow);

            await tvShowRepository
                .SaveChangesAsync();
        }
        catch
        {
            fileStorage.Delete(
                newPoster,
                "tvshows");

            throw;
        }
    }

    public async Task UpdateAsync(
        Guid id,
        TVShowUpdateRequest request)
    {
        var tvShow =
            await tvShowRepository
                .FirstOrDefaultAsync(
                    x => x.Id == id,
                    true,
                    "TVShowDetail",
                    "TVShowGenres",
                    "TVShowActors");

        if (tvShow is null)
            throw new NotFoundException(
                "TV show was not found.");

        await ValidateRelatedEntitiesAsync(
            request.GenreIds,
            request.Actors);

        var oldPoster =
            tvShow.PosterUrl;

        mapper.Map(
            request,
            tvShow);

        if (tvShow.TVShowDetail is null)
        {
            tvShow.TVShowDetail =
                mapper.Map<TVShowDetail>(
                    request);
        }
        else
        {
            mapper.Map(
                request,
                tvShow.TVShowDetail);
        }

        SetRelationships(
            tvShow,
            request.GenreIds,
            request.Actors);

        string? newPoster = null;

        if (request.PosterImage is not null)
        {
            newPoster =
                await fileStorage.SaveAsync(
                    request.PosterImage,
                    "tvshows");

            tvShow.PosterUrl =
                newPoster;
        }

        try
        {
            tvShowRepository.Update(
                tvShow);

            await tvShowRepository
                .SaveChangesAsync();
        }
        catch
        {
            fileStorage.Delete(
                newPoster,
                "tvshows");

            throw;
        }

        if (newPoster is not null)
        {
            fileStorage.Delete(
                oldPoster,
                "tvshows");
        }
    }

    public async Task DeleteAsync(
        Guid id)
    {
        var tvShow =
            await tvShowRepository
                .FirstOrDefaultAsync(
                    x => x.Id == id,
                    true,
                    "Seasons.Episodes");

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

        fileStorage.Delete(
            poster,
            "tvshows");

        foreach (var image
                 in episodeImages)
        {
            fileStorage.Delete(
                image,
                "episodes");
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
                .Select(genreId =>
                    new TVShowGenre
                    {
                        GenreId = genreId
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
            genreRepository,
            genreIds,
            "Genre");

        await EnsureIdsExistAsync(
            actorRepository,
            actors.Select(x =>
                x.ActorId),
            "Actor");
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
                .Select(x => x.Id)
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
}