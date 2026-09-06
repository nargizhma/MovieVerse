using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MovieVerse.Dtos.Directors;
using MovieVerse.Dtos.People;
using MovieVerse.Exceptions;
using MovieVerse.Extensions;
using MovieVerse.Models;
using MovieVerse.Repositories.Interfaces;
using MovieVerse.Services.Interfaces;

namespace MovieVerse.Services;

public class DirectorService(
    IGenericRepository<Director> repository,
    IGenericRepository<MovieDirector> movieDirectorRepository,
    IGenericRepository<EpisodeDirector> episodeDirectorRepository,
    IMapper mapper,
    IWebHostEnvironment environment,
    IHttpContextAccessor httpContextAccessor)
    : IDirectorService
{
    public async Task<List<DirectorReturnDto>> GetAllAsync()
    {
        var directors = await repository.Query()
            .Include(x => x.DirectorDetail)
            .AsNoTracking()
            .ToListAsync();

        return mapper.Map<List<DirectorReturnDto>>(
            directors);
    }

    public async Task<DirectorDetailsDto> GetByIdAsync(
        Guid id)
    {
        var director = await repository.Query()
            .Include(x => x.DirectorDetail)
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);

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
        DirectorCreateDto dto)
    {
        var director =
            mapper.Map<Director>(dto);

        var folderPath =
            environment.GetImageFolderPath(
                "directors");

        string? newImage = null;

        try
        {
            if (dto.ProfileImage is not null)
            {
                newImage =
                    await dto.ProfileImage
                        .SaveFileAsync(
                            folderPath);

                director.ProfileImageUrl =
                    newImage;
            }

            await repository.AddAsync(director);

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
        DirectorUpdateDto dto)
    {
        var director = await repository.Query()
            .Include(x => x.DirectorDetail)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (director is null)
            throw new NotFoundException(
                "Director was not found.");

        var oldImage =
            director.ProfileImageUrl;

        mapper.Map(dto, director);

        if (director.DirectorDetail is null)
        {
            director.DirectorDetail =
                mapper.Map<DirectorDetail>(dto);
        }
        else
        {
            mapper.Map(
                dto,
                director.DirectorDetail);
        }

        var folderPath =
            environment.GetImageFolderPath(
                "directors");

        string? newImage = null;

        if (dto.ProfileImage is not null)
        {
            newImage =
                await dto.ProfileImage
                    .SaveFileAsync(
                        folderPath);

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
        var director =
            await repository.GetByIdAsync(id);

        if (director is null)
            throw new NotFoundException(
                "Director was not found.");

        var isUsed =
            await movieDirectorRepository.Query()
                .AnyAsync(x =>
                    x.DirectorId == id)
            ||
            await episodeDirectorRepository.Query()
                .AnyAsync(x =>
                    x.DirectorId == id);

        if (isUsed)
            throw new ConflictException(
                "Director cannot be deleted because they are used in existing titles.");

        var imageName =
            director.ProfileImageUrl;

        repository.Delete(director);

        await repository.SaveChangesAsync();

        FileManager.DeleteFile(
            imageName,
            environment.GetImageFolderPath("directors"));
    }

    private async Task<List<FilmographyItemDto>>
        GetFilmographyAsync(
            Guid directorId)
    {
        var movieCredits =
            await movieDirectorRepository.Query()
                .Where(x =>
                    x.DirectorId == directorId)
                .Include(x =>
                    x.Movie)
                .AsNoTracking()
                .ToListAsync();

        var episodeCredits =
            await episodeDirectorRepository.Query()
                .Where(x =>
                    x.DirectorId == directorId)
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

                        EpisodeCount =
                            group
                                .Select(x =>
                                    x.EpisodeId)
                                .Distinct()
                                .Count()
                    };
                });

        filmography.AddRange(
            tvShowCredits);

        return filmography
            .OrderByDescending(x =>
                x.ReleaseYear)
            .ThenBy(x =>
                x.Title)
            .ToList();
    }


}
