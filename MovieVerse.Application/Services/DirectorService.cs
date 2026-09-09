using AutoMapper;
using MovieVerse.Abstractions.Media;
using MovieVerse.Dtos.Directors;
using MovieVerse.Dtos.People;
using MovieVerse.Exceptions;
using MovieVerse.Models;
using MovieVerse.Repositories.Interfaces;
using MovieVerse.Requests.Directors;
using MovieVerse.Services.Interfaces;

namespace MovieVerse.Services;

public class DirectorService(
    IGenericRepository<Director> repository,
    IGenericRepository<MovieDirector> movieDirectorRepository,
    IGenericRepository<EpisodeDirector> episodeDirectorRepository,
    IMapper mapper,
    IFileStorage fileStorage,
    IMediaUrlBuilder mediaUrlBuilder)
    : IDirectorService
{
    public async Task<List<DirectorReturnDto>> GetAllAsync()
    {
        var directors =
            await repository.FindAllAsync(
                x => true,
                false,
                "DirectorDetail");

        return mapper.Map<List<DirectorReturnDto>>(
            directors);
    }

    public async Task<DirectorDetailsDto> GetByIdAsync(
        Guid id)
    {
        var director =
            await repository.FirstOrDefaultAsync(
                x => x.Id == id,
                false,
                "DirectorDetail");

        if (director is null)
            throw new NotFoundException(
                "Director was not found.");

        var result =
            mapper.Map<DirectorDetailsDto>(
                director);

        result.Filmography =
            await GetFilmographyAsync(id);

        return result;
    }

    public async Task CreateAsync(
        DirectorCreateRequest request)
    {
        var director =
            mapper.Map<Director>(request);

        string? newImage = null;

        try
        {
            if (request.ProfileImage is not null)
            {
                newImage =
                    await fileStorage.SaveAsync(
                        request.ProfileImage,
                        "directors");

                director.ProfileImageUrl =
                    newImage;
            }

            await repository.AddAsync(
                director);

            await repository.SaveChangesAsync();
        }
        catch
        {
            fileStorage.Delete(
                newImage,
                "directors");

            throw;
        }
    }

    public async Task UpdateAsync(
        Guid id,
        DirectorUpdateRequest request)
    {
        var director =
            await repository.FirstOrDefaultAsync(
                x => x.Id == id,
                true,
                "DirectorDetail");

        if (director is null)
            throw new NotFoundException(
                "Director was not found.");

        var oldImage =
            director.ProfileImageUrl;

        mapper.Map(
            request,
            director);

        if (director.DirectorDetail is null)
        {
            director.DirectorDetail =
                mapper.Map<DirectorDetail>(
                    request);
        }
        else
        {
            mapper.Map(
                request,
                director.DirectorDetail);
        }

        string? newImage = null;

        if (request.ProfileImage is not null)
        {
            newImage =
                await fileStorage.SaveAsync(
                    request.ProfileImage,
                    "directors");

            director.ProfileImageUrl =
                newImage;
        }

        try
        {
            repository.Update(director);

            await repository.SaveChangesAsync();
        }
        catch
        {
            fileStorage.Delete(
                newImage,
                "directors");

            throw;
        }

        if (newImage is not null)
        {
            fileStorage.Delete(
                oldImage,
                "directors");
        }
    }

    public async Task DeleteAsync(Guid id)
    {
        var director =
            await repository.GetByIdAsync(id);

        if (director is null)
            throw new NotFoundException(
                "Director was not found.");

        var isUsed =
            await movieDirectorRepository.AnyAsync(
                x => x.DirectorId == id)
            ||
            await episodeDirectorRepository.AnyAsync(
                x => x.DirectorId == id);

        if (isUsed)
            throw new ConflictException(
                "Director cannot be deleted because they are used in existing titles.");

        var imageName =
            director.ProfileImageUrl;

        repository.Delete(director);

        await repository.SaveChangesAsync();

        fileStorage.Delete(
            imageName,
            "directors");
    }

    private async Task<List<FilmographyItemDto>>
        GetFilmographyAsync(
            Guid directorId)
    {
        var movieCredits =
            await movieDirectorRepository.FindAllAsync(
                x => x.DirectorId == directorId,
                false,
                "Movie");

        var episodeCredits =
            await episodeDirectorRepository.FindAllAsync(
                x => x.DirectorId == directorId,
                false,
                "Episode.Season.TVShow");

        var filmography =
            new List<FilmographyItemDto>();

        filmography.AddRange(
            movieCredits.Select(x =>
                new FilmographyItemDto
                {
                    Id = x.MovieId,
                    ContentType = "Movie",
                    Title = x.Movie.Title,
                    ReleaseYear =
                        x.Movie.ReleaseDate.Year,

                    PosterUrl =
                        mediaUrlBuilder.BuildImageUrl(
                            x.Movie.PosterUrl,
                            "movies")
                }));

        var tvShowCredits =
            episodeCredits
                .GroupBy(x =>
                    x.Episode.Season.TVShowId)
                .Select(group =>
                {
                    var first =
                        group.First();

                    var tvShow =
                        first.Episode
                            .Season
                            .TVShow;

                    return new FilmographyItemDto
                    {
                        Id = tvShow.Id,
                        ContentType = "TVShow",
                        Title = tvShow.Title,

                        ReleaseYear =
                            tvShow.ReleaseDate.Year,

                        PosterUrl =
                            mediaUrlBuilder.BuildImageUrl(
                                tvShow.PosterUrl,
                                "tvshows"),

                        EpisodeCount =
                            group
                                .Select(x => x.EpisodeId)
                                .Distinct()
                                .Count()
                    };
                });

        filmography.AddRange(tvShowCredits);

        return filmography
            .OrderByDescending(x =>
                x.ReleaseYear)
            .ThenBy(x => x.Title)
            .ToList();
    }
}