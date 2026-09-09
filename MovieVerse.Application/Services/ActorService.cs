using AutoMapper;
using MovieVerse.Abstractions.Media;
using MovieVerse.Dtos.Actors;
using MovieVerse.Dtos.People;
using MovieVerse.Exceptions;
using MovieVerse.Models;
using MovieVerse.Repositories.Interfaces;
using MovieVerse.Requests.Actors;
using MovieVerse.Services.Interfaces;

namespace MovieVerse.Services;

public class ActorService(
    IGenericRepository<Actor> repository,
    IGenericRepository<MovieActor> movieActorRepository,
    IGenericRepository<TVShowActor> tvShowActorRepository,
    IGenericRepository<EpisodeActor> episodeActorRepository,
    IFileStorage fileStorage,
    IMediaUrlBuilder mediaUrlBuilder,
    IMapper mapper)
    : IActorService
{
    public async Task<List<ActorReturnDto>>
        GetAllAsync()
    {
        var actors =
            await repository.FindAllAsync(
                x => true,
                false,
                "ActorDetail");

        return mapper.Map<
            List<ActorReturnDto>>(
                actors);
    }

    public async Task<ActorDetailsDto>
        GetByIdAsync(
            Guid id)
    {
        var actor =
            await repository.FirstOrDefaultAsync(
                x => x.Id == id,
                false,
                "ActorDetail");

        if (actor is null)
            throw new NotFoundException(
                "Actor was not found.");

        var result =
            mapper.Map<ActorDetailsDto>(
                actor);

        result.Filmography =
            await GetFilmographyAsync(
                id);

        return result;
    }

    public async Task CreateAsync(
        ActorCreateRequest request)
    {
        var actor =
            mapper.Map<Actor>(
                request);

        string? newImage = null;

        try
        {
            if (request.ProfileImage is not null)
            {
                newImage =
                    await fileStorage.SaveAsync(
                        request.ProfileImage,
                        "actors");

                actor.ProfileImageUrl =
                    newImage;
            }

            await repository.AddAsync(
                actor);

            await repository.SaveChangesAsync();
        }
        catch
        {
            fileStorage.Delete(
                newImage,
                "actors");

            throw;
        }
    }

    public async Task UpdateAsync(
        Guid id,
        ActorUpdateRequest request)
    {
        var actor =
            await repository.FirstOrDefaultAsync(
                x => x.Id == id,
                true,
                "ActorDetail");

        if (actor is null)
            throw new NotFoundException(
                "Actor was not found.");

        var oldImage =
            actor.ProfileImageUrl;

        mapper.Map(
            request,
            actor);

        if (actor.ActorDetail is null)
        {
            actor.ActorDetail =
                mapper.Map<ActorDetail>(
                    request);
        }
        else
        {
            mapper.Map(
                request,
                actor.ActorDetail);
        }

        string? newImage = null;

        if (request.ProfileImage is not null)
        {
            newImage =
                await fileStorage.SaveAsync(
                    request.ProfileImage,
                    "actors");

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
            fileStorage.Delete(
                newImage,
                "actors");

            throw;
        }

        if (newImage is not null)
        {
            fileStorage.Delete(
                oldImage,
                "actors");
        }
    }

    public async Task DeleteAsync(
        Guid id)
    {
        var actor =
            await repository.GetByIdAsync(
                id);

        if (actor is null)
            throw new NotFoundException(
                "Actor was not found.");

        var isUsed =
            await movieActorRepository.AnyAsync(
                x => x.ActorId == id)
            ||
            await tvShowActorRepository.AnyAsync(
                x => x.ActorId == id)
            ||
            await episodeActorRepository.AnyAsync(
                x => x.ActorId == id);

        if (isUsed)
            throw new ConflictException(
                "Actor cannot be deleted because they are used in existing titles.");

        var imageName =
            actor.ProfileImageUrl;

        repository.Delete(actor);

        await repository.SaveChangesAsync();

        fileStorage.Delete(
            imageName,
            "actors");
    }

    private async Task<List<FilmographyItemDto>>
        GetFilmographyAsync(
            Guid actorId)
    {
        var movieCredits =
            await movieActorRepository.FindAllAsync(
                x => x.ActorId == actorId,
                false,
                "Movie");

        var tvShowCredits =
            await tvShowActorRepository.FindAllAsync(
                x => x.ActorId == actorId,
                false,
                "TVShow");

        var episodeCredits =
            await episodeActorRepository.FindAllAsync(
                x => x.ActorId == actorId,
                false,
                "Episode.Season.TVShow");

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
                        mediaUrlBuilder
                            .BuildImageUrl(
                                x.Movie.PosterUrl,
                                "movies"),

                    CharacterName =
                        x.CharacterName
                }));

        var tvShowItems =
            new Dictionary<
                Guid,
                FilmographyItemDto>();

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
                        credit.TVShow
                            .ReleaseDate.Year,

                    PosterUrl =
                        mediaUrlBuilder
                            .BuildImageUrl(
                                credit.TVShow
                                    .PosterUrl,
                                "tvshows"),

                    CharacterName =
                        credit.CharacterName
                };
        }

        var groupedEpisodeCredits =
            episodeCredits.GroupBy(x =>
                x.Episode
                    .Season
                    .TVShowId);

        foreach (var group
                 in groupedEpisodeCredits)
        {
            var first =
                group.First();

            var tvShow =
                first.Episode
                    .Season
                    .TVShow;

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
                        !string.IsNullOrWhiteSpace(
                            x))
                    .Select(x => x!)
                    .Distinct(
                        StringComparer
                            .OrdinalIgnoreCase)
                    .ToList();

            if (tvShowItems.TryGetValue(
                    tvShow.Id,
                    out var existingItem))
            {
                existingItem.EpisodeCount =
                    episodeCount;

                existingItem.CharacterName =
                    MergeCharacterNames(
                        existingItem
                            .CharacterName,
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
                        mediaUrlBuilder
                            .BuildImageUrl(
                                tvShow.PosterUrl,
                                "tvshows"),

                    CharacterName =
                        episodeCharacterNames
                            .Count == 0
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

    private static string?
        MergeCharacterNames(
            string? primaryCharacterName,
            IEnumerable<string>
                additionalNames)
    {
        var names =
            new List<string>();

        if (!string.IsNullOrWhiteSpace(
                primaryCharacterName))
        {
            names.Add(
                primaryCharacterName);
        }

        foreach (var name
                 in additionalNames)
        {
            if (names.Any(x =>
                    string.Equals(
                        x,
                        name,
                        StringComparison
                            .OrdinalIgnoreCase)))
            {
                continue;
            }

            names.Add(name);
        }

        return names.Count == 0
            ? null
            : string.Join(
                ", ",
                names);
    }
}