using AutoMapper;
using MovieVerse.Abstractions.Media;
using MovieVerse.Dtos.Common;
using MovieVerse.Dtos.Movies;
using MovieVerse.Exceptions;
using MovieVerse.Models;
using MovieVerse.Models.Common;
using MovieVerse.Repositories.Interfaces;
using MovieVerse.Requests.Movies;
using MovieVerse.Services.Interfaces;

namespace MovieVerse.Services;

public class MovieService(
    IGenericRepository<Movie> movieRepository,
    IGenericRepository<Genre> genreRepository,
    IGenericRepository<Actor> actorRepository,
    IGenericRepository<Director> directorRepository,
    IGenericRepository<Writer> writerRepository,
    IMapper mapper,
    IFileStorage fileStorage)
    : IMovieService
{
    public async Task<PagedResultDto<MovieReturnDto>>
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

        var movies =
            await movieRepository.FindAllAsync(
                x =>
                    (search == null ||
                     x.Title.ToLower()
                         .Contains(search))
                    &&
                    (!genreId.HasValue ||
                     x.MovieGenres.Any(g =>
                         g.GenreId ==
                         genreId.Value))
                    &&
                    (!releaseYear.HasValue ||
                     x.ReleaseDate.Year ==
                     releaseYear.Value)
                    &&
                    (!actorId.HasValue ||
                     x.MovieActors.Any(a =>
                         a.ActorId ==
                         actorId.Value))
                    &&
                    (!directorId.HasValue ||
                     x.MovieDirectors.Any(d =>
                         d.DirectorId ==
                         directorId.Value))
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
                "MovieGenres.Genre",
                "Reviews");

        IEnumerable<Movie> sorted =
            ApplySorting(
                movies,
                filter);

        var totalCount =
            movies.Count;

        var pageItems =
            sorted
                .Skip(
                    (filter.PageNumber - 1) *
                    filter.PageSize)
                .Take(filter.PageSize)
                .ToList();

        return new PagedResultDto<MovieReturnDto>
        {
            Items =
                mapper.Map<List<MovieReturnDto>>(
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

    private static IEnumerable<Movie>
        ApplySorting(
            IEnumerable<Movie> movies,
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
                    ? movies.OrderByDescending(
                        x => x.Title)
                    : movies.OrderBy(
                        x => x.Title),

            "rating" =>
                filter.SortDescending
                    ? movies.OrderByDescending(
                        GetAverageRating)
                    : movies.OrderBy(
                        GetAverageRating),

            "year" =>
                filter.SortDescending
                    ? movies.OrderByDescending(
                        x => x.ReleaseDate)
                    : movies.OrderBy(
                        x => x.ReleaseDate),

            _ =>
                movies.OrderByDescending(
                    x => x.ReleaseDate)
        };
    }

    private static decimal GetAverageRating(
        Movie movie)
    {
        return movie.Reviews.Count == 0
            ? 0m
            : movie.Reviews.Average(
                x => x.Rating);
    }

    public async Task<MovieDetailsDto>
        GetByIdAsync(
            Guid id)
    {
        var movie =
            await movieRepository
                .FirstOrDefaultAsync(
                    x => x.Id == id,
                    false,
                    "MovieDetail",
                    "MovieGenres.Genre",
                    "MovieActors.Actor",
                    "MovieDirectors.Director",
                    "MovieWriters.Writer",
                    "Reviews");

        if (movie is null)
            throw new NotFoundException(
                "Movie was not found.");

        return mapper.Map<MovieDetailsDto>(
            movie);
    }

    public async Task CreateAsync(
        MovieCreateRequest request)
    {
        await ValidateRelatedEntitiesAsync(
            request.GenreIds,
            request.Actors,
            request.DirectorIds,
            request.WriterIds);

        var movie =
            mapper.Map<Movie>(request);

        movie.MovieDetail =
            mapper.Map<MovieDetail>(
                request);

        SetRelationships(
            movie,
            request.GenreIds,
            request.Actors,
            request.DirectorIds,
            request.WriterIds);

        string? newPoster = null;

        try
        {
            if (request.PosterImage is not null)
            {
                newPoster =
                    await fileStorage.SaveAsync(
                        request.PosterImage,
                        "movies");

                movie.PosterUrl =
                    newPoster;
            }

            await movieRepository.AddAsync(
                movie);

            await movieRepository
                .SaveChangesAsync();
        }
        catch
        {
            fileStorage.Delete(
                newPoster,
                "movies");

            throw;
        }
    }

    public async Task UpdateAsync(
        Guid id,
        MovieUpdateRequest request)
    {
        var movie =
            await movieRepository
                .FirstOrDefaultAsync(
                    x => x.Id == id,
                    true,
                    "MovieDetail",
                    "MovieGenres",
                    "MovieActors",
                    "MovieDirectors",
                    "MovieWriters");

        if (movie is null)
            throw new NotFoundException(
                "Movie was not found.");

        await ValidateRelatedEntitiesAsync(
            request.GenreIds,
            request.Actors,
            request.DirectorIds,
            request.WriterIds);

        var oldPoster =
            movie.PosterUrl;

        mapper.Map(
            request,
            movie);

        if (movie.MovieDetail is null)
        {
            movie.MovieDetail =
                mapper.Map<MovieDetail>(
                    request);
        }
        else
        {
            mapper.Map(
                request,
                movie.MovieDetail);
        }

        SetRelationships(
            movie,
            request.GenreIds,
            request.Actors,
            request.DirectorIds,
            request.WriterIds);

        string? newPoster = null;

        if (request.PosterImage is not null)
        {
            newPoster =
                await fileStorage.SaveAsync(
                    request.PosterImage,
                    "movies");

            movie.PosterUrl =
                newPoster;
        }

        try
        {
            movieRepository.Update(movie);

            await movieRepository
                .SaveChangesAsync();
        }
        catch
        {
            fileStorage.Delete(
                newPoster,
                "movies");

            throw;
        }

        if (newPoster is not null)
        {
            fileStorage.Delete(
                oldPoster,
                "movies");
        }
    }

    public async Task DeleteAsync(
        Guid id)
    {
        var movie =
            await movieRepository
                .GetByIdAsync(id);

        if (movie is null)
            throw new NotFoundException(
                "Movie was not found.");

        var poster =
            movie.PosterUrl;

        movieRepository.Delete(movie);

        await movieRepository
            .SaveChangesAsync();

        fileStorage.Delete(
            poster,
            "movies");
    }

    private static void SetRelationships(
        Movie movie,
        IEnumerable<Guid> genreIds,
        IEnumerable<MovieActorInputDto> actors,
        IEnumerable<Guid> directorIds,
        IEnumerable<Guid> writerIds)
    {
        movie.MovieGenres.Clear();

        movie.MovieGenres.AddRange(
            genreIds
                .Distinct()
                .Select(genreId =>
                    new MovieGenre
                    {
                        GenreId = genreId
                    }));

        movie.MovieActors.Clear();

        movie.MovieActors.AddRange(
            actors.Select(actor =>
                new MovieActor
                {
                    ActorId =
                        actor.ActorId,

                    CharacterName =
                        actor.CharacterName,

                    CastOrder =
                        actor.CastOrder
                }));

        movie.MovieDirectors.Clear();

        movie.MovieDirectors.AddRange(
            directorIds
                .Distinct()
                .Select(directorId =>
                    new MovieDirector
                    {
                        DirectorId =
                            directorId
                    }));

        movie.MovieWriters.Clear();

        movie.MovieWriters.AddRange(
            writerIds
                .Distinct()
                .Select(writerId =>
                    new MovieWriter
                    {
                        WriterId =
                            writerId
                    }));
    }

    private async Task
        ValidateRelatedEntitiesAsync(
            IEnumerable<Guid> genreIds,
            IEnumerable<MovieActorInputDto> actors,
            IEnumerable<Guid> directorIds,
            IEnumerable<Guid> writerIds)
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