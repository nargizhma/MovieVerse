using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MovieVerse.Dtos.Actors;
using MovieVerse.Dtos.People;
using MovieVerse.Exceptions;
using MovieVerse.Extensions;
using MovieVerse.Models;
using MovieVerse.Repositories.Interfaces;
using MovieVerse.Services.Interfaces;

namespace MovieVerse.Services;

public class ActorService(
    IGenericRepository<Actor> repository,
    IGenericRepository<MovieActor> movieActorRepository,
    IGenericRepository<TVShowActor> tvShowActorRepository,
    IGenericRepository<EpisodeActor> episodeActorRepository,
    IWebHostEnvironment environment,
    IHttpContextAccessor httpContextAccessor,
    IMapper mapper)
    : IActorService
{
    public async Task<List<ActorReturnDto>> GetAllAsync()
    {
        var actors = await repository.Query()
            .Include(x => x.ActorDetail)
            .AsNoTracking()
            .ToListAsync();

        return mapper.Map<List<ActorReturnDto>>(actors);
    }

    public async Task<ActorDetailsDto> GetByIdAsync(
        Guid id)
    {
        var actor = await repository.Query()
            .Include(x => x.ActorDetail)
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);

        if (actor is null)
            throw new NotFoundException(
                "Actor was not found.");

        var result =
            mapper.Map<ActorDetailsDto>(actor);

        result.Filmography =
            await GetFilmographyAsync(id);

        return result;
    }

    public async Task CreateAsync(
        ActorCreateDto dto)
    {
        var actor =
            mapper.Map<Actor>(dto);

        var folderPath =
            environment.GetImageFolderPath(
                "actors");

        string? newImage = null;

        try
        {
            if (dto.ProfileImage is not null)
            {
                newImage =
                    await dto.ProfileImage
                        .SaveFileAsync(
                            folderPath);

                actor.ProfileImageUrl =
                    newImage;
            }

            await repository.AddAsync(actor);

            await repository.SaveChangesAsync();
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
        ActorUpdateDto dto)
    {
        var actor = await repository.Query()
            .Include(x => x.ActorDetail)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (actor is null)
            throw new NotFoundException(
                "Actor was not found.");

        var oldImage =
            actor.ProfileImageUrl;

        mapper.Map(dto, actor);

        if (actor.ActorDetail is null)
        {
            actor.ActorDetail =
                mapper.Map<ActorDetail>(dto);
        }
        else
        {
            mapper.Map(
                dto,
                actor.ActorDetail);
        }

        var folderPath =
            environment.GetImageFolderPath(
                "actors");

        string? newImage = null;

        if (dto.ProfileImage is not null)
        {
            newImage =
                await dto.ProfileImage
                    .SaveFileAsync(
                        folderPath);

            actor.ProfileImageUrl =
                newImage;
        }

        try
        {
            repository.Update(actor);

            await repository.SaveChangesAsync();
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

    public async Task DeleteAsync(Guid id)
    {
        var actor =
            await repository.GetByIdAsync(id);

        if (actor is null)
            throw new NotFoundException(
                "Actor was not found.");

        var isUsed =
            await movieActorRepository.Query()
                .AnyAsync(x => x.ActorId == id)
            ||
            await tvShowActorRepository.Query()
                .AnyAsync(x => x.ActorId == id)
            ||
            await episodeActorRepository.Query()
                .AnyAsync(x => x.ActorId == id);

        if (isUsed)
            throw new ConflictException(
                "Actor cannot be deleted because they are used in existing titles.");

        var imageName =
            actor.ProfileImageUrl;

        repository.Delete(actor);

        await repository.SaveChangesAsync();

        FileManager.DeleteFile(
            imageName,
            environment.GetImageFolderPath("actors"));
    }

    private async Task<List<FilmographyItemDto>>
        GetFilmographyAsync(
            Guid actorId)
    {
        var movieCredits =
            await movieActorRepository.Query()
                .Where(x =>
                    x.ActorId == actorId)
                .Include(x =>
                    x.Movie)
                .AsNoTracking()
                .ToListAsync();

        var tvShowCredits =
            await tvShowActorRepository.Query()
                .Where(x =>
                    x.ActorId == actorId)
                .Include(x =>
                    x.TVShow)
                .AsNoTracking()
                .ToListAsync();

        var episodeCredits =
            await episodeActorRepository.Query()
                .Where(x =>
                    x.ActorId == actorId)
                .Include(x =>
                    x.Episode)
                    .ThenInclude(x =>
                        x.Season)
                    .ThenInclude(x =>
                        x.TVShow)
                .AsNoTracking()
                .ToListAsync();

        var filmography =
            new List<FilmographyItemDto>();

        filmography.AddRange(
            movieCredits.Select(x =>
                new FilmographyItemDto
                {
                    Id =
                        x.MovieId,

                    ContentType =
                        "Movie",

                    Title =
                        x.Movie.Title,

                    ReleaseYear =
                        x.Movie.ReleaseDate.Year,

                    PosterUrl =
                        httpContextAccessor.BuildImageUrl(
                            x.Movie.PosterUrl,
                            "movies"),

                    CharacterName =
                        x.CharacterName
                }));

        var tvShowItems =
            new Dictionary<Guid, FilmographyItemDto>();

        foreach (var credit in tvShowCredits)
        {
            tvShowItems[credit.TVShowId] =
                new FilmographyItemDto
                {
                    Id =
                        credit.TVShowId,

                    ContentType =
                        "TVShow",

                    Title =
                        credit.TVShow.Title,

                    ReleaseYear =
                        credit.TVShow.ReleaseDate.Year,

                    PosterUrl =
                        httpContextAccessor.BuildImageUrl(
                            credit.TVShow.PosterUrl,
                            "tvshows"),

                    CharacterName =
                        credit.CharacterName
                };
        }

        var groupedEpisodeCredits =
            episodeCredits
                .GroupBy(x =>
                    x.Episode.Season.TVShowId);

        foreach (var group in groupedEpisodeCredits)
        {
            var first =
                group.First();

            var tvShow =
                first.Episode.Season.TVShow;

            var episodeCount =
                group
                    .Select(x =>
                        x.EpisodeId)
                    .Distinct()
                    .Count();

            var episodeCharacterNames =
                group
                    .Select(x =>
                        x.CharacterName)
                    .Where(x =>
                        !string.IsNullOrWhiteSpace(x))
                    .Select(x =>
                        x!)
                    .Distinct(
                        StringComparer.OrdinalIgnoreCase)
                    .ToList();

            if (tvShowItems.TryGetValue(
                    tvShow.Id,
                    out var existingItem))
            {
                existingItem.EpisodeCount =
                    episodeCount;

                existingItem.CharacterName =
                    MergeCharacterNames(
                        existingItem.CharacterName,
                        episodeCharacterNames);

                continue;
            }

            tvShowItems[tvShow.Id] =
                new FilmographyItemDto
                {
                    Id =
                        tvShow.Id,

                    ContentType =
                        "TVShow",

                    Title =
                        tvShow.Title,

                    ReleaseYear =
                        tvShow.ReleaseDate.Year,

                    PosterUrl =
                        httpContextAccessor.BuildImageUrl(
                            tvShow.PosterUrl,
                            "tvshows"),

                    CharacterName =
                        episodeCharacterNames.Count == 0
                            ? null
                            : string.Join(
                                ", ",
                                episodeCharacterNames),

                    EpisodeCount =
                        episodeCount
                };
        }

        filmography.AddRange(
            tvShowItems.Values);

        return filmography
            .OrderByDescending(x =>
                x.ReleaseYear)
            .ThenBy(x =>
                x.Title)
            .ToList();
    }

    private static string? MergeCharacterNames(
        string? primaryCharacterName,
        IEnumerable<string> additionalNames)
    {
        var names =
            new List<string>();

        if (!string.IsNullOrWhiteSpace(
                primaryCharacterName))
        {
            names.Add(primaryCharacterName);
        }

        foreach (var name in additionalNames)
        {
            if (names.Any(x =>
                    string.Equals(
                        x,
                        name,
                        StringComparison.OrdinalIgnoreCase)))
            {
                continue;
            }

            names.Add(name);
        }

        return names.Count == 0
            ? null
            : string.Join(", ", names);
    }


}
