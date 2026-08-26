using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MovieVerse.Dtos.Movies;
using MovieVerse.Exceptions;
using MovieVerse.Extensions;
using MovieVerse.Models;
using MovieVerse.Models.Common;
using MovieVerse.Repositories.Interfaces;
using MovieVerse.Services.Interfaces;

namespace MovieVerse.Services;

public class MovieService(
    IGenericRepository<Movie> movieRepository,
    IGenericRepository<Genre> genreRepository,
    IGenericRepository<Actor> actorRepository,
    IGenericRepository<Director> directorRepository,
    IGenericRepository<Writer> writerRepository,
    IMapper mapper,
    IWebHostEnvironment environment)
    : IMovieService
{
    public async Task<List<MovieReturnDto>> GetAllAsync()
    {
        var movies = await movieRepository.Query()
            .Include(x => x.MovieGenres)
                .ThenInclude(x => x.Genre)
            .Include(x => x.Reviews)
            .AsNoTracking()
            .ToListAsync();

        return mapper.Map<List<MovieReturnDto>>(
            movies);
    }

    public async Task<MovieDetailsDto> GetByIdAsync(
        Guid id)
    {
        var movie = await movieRepository.Query()
            .Include(x => x.MovieDetail)

            .Include(x => x.MovieGenres)
                .ThenInclude(x => x.Genre)

            .Include(x => x.MovieActors)
                .ThenInclude(x => x.Actor)

            .Include(x => x.MovieDirectors)
                .ThenInclude(x => x.Director)

            .Include(x => x.MovieWriters)
                .ThenInclude(x => x.Writer)

            .Include(x => x.Reviews)

            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);

        if (movie is null)
            throw new NotFoundException(
                "Movie was not found.");

        return mapper.Map<MovieDetailsDto>(
            movie);
    }

    public async Task CreateAsync(
        MovieCreateDto dto)
    {
        await ValidateRelatedEntitiesAsync(
            dto.GenreIds,
            dto.Actors,
            dto.DirectorIds,
            dto.WriterIds);

        var movie =
            mapper.Map<Movie>(dto);

        movie.MovieDetail =
            mapper.Map<MovieDetail>(dto);

        SetRelationships(
            movie,
            dto.GenreIds,
            dto.Actors,
            dto.DirectorIds,
            dto.WriterIds);

        if (dto.PosterImage is not null)
        {
            movie.PosterUrl =
                await dto.PosterImage.SaveFileAsync(
                    GetImageFolderPath());
        }

        await movieRepository.AddAsync(movie);

        await movieRepository.SaveChangesAsync();
    }

    public async Task UpdateAsync(
        Guid id,
        MovieUpdateDto dto)
    {
        var movie = await movieRepository.Query()

            .Include(x => x.MovieDetail)

            .Include(x => x.MovieGenres)

            .Include(x => x.MovieActors)

            .Include(x => x.MovieDirectors)

            .Include(x => x.MovieWriters)

            .FirstOrDefaultAsync(x => x.Id == id);

        if (movie is null)
            throw new NotFoundException(
                "Movie was not found.");

        await ValidateRelatedEntitiesAsync(
            dto.GenreIds,
            dto.Actors,
            dto.DirectorIds,
            dto.WriterIds);

        var oldPoster =
            movie.PosterUrl;

        mapper.Map(dto, movie);

        if (movie.MovieDetail is null)
        {
            movie.MovieDetail =
                mapper.Map<MovieDetail>(dto);
        }
        else
        {
            mapper.Map(
                dto,
                movie.MovieDetail);
        }

        SetRelationships(
            movie,
            dto.GenreIds,
            dto.Actors,
            dto.DirectorIds,
            dto.WriterIds);

        if (dto.PosterImage is not null)
        {
            movie.PosterUrl =
                await dto.PosterImage.SaveFileAsync(
                    GetImageFolderPath());
        }

        movieRepository.Update(movie);

        await movieRepository.SaveChangesAsync();

        if (dto.PosterImage is not null)
        {
            FileManager.DeleteFile(
                oldPoster,
                GetImageFolderPath());
        }
    }

    public async Task DeleteAsync(Guid id)
    {
        var movie =
            await movieRepository.GetByIdAsync(id);

        if (movie is null)
            throw new NotFoundException(
                "Movie was not found.");

        var poster =
            movie.PosterUrl;

        movieRepository.Delete(movie);

        await movieRepository.SaveChangesAsync();

        FileManager.DeleteFile(
            poster,
            GetImageFolderPath());
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
                    ActorId = actor.ActorId,
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

    private async Task ValidateRelatedEntitiesAsync(
        IEnumerable<Guid> genreIds,
        IEnumerable<MovieActorInputDto> actors,
        IEnumerable<Guid> directorIds,
        IEnumerable<Guid> writerIds)
    {
        await EnsureIdsExistAsync(
            genreRepository.Query(),
            genreIds,
            "Genre");

        await EnsureIdsExistAsync(
            actorRepository.Query(),
            actors.Select(x => x.ActorId),
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

        var existingIds = await query
            .Where(x =>
                requestedIds.Contains(x.Id))
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
            "movies");
    }
}