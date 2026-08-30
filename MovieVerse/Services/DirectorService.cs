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

        if (dto.ProfileImage is not null)
        {
            director.ProfileImageUrl =
                await dto.ProfileImage.SaveFileAsync(
                    GetImageFolderPath());
        }

        await repository.AddAsync(director);

        await repository.SaveChangesAsync();
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

        if (dto.ProfileImage is not null)
        {
            director.ProfileImageUrl =
                await dto.ProfileImage.SaveFileAsync(
                    GetImageFolderPath());
        }

        repository.Update(director);

        await repository.SaveChangesAsync();

        if (dto.ProfileImage is not null)
        {
            FileManager.DeleteFile(
                oldImage,
                GetImageFolderPath());
        }
    }

    public async Task DeleteAsync(Guid id)
    {
        var director =
            await repository.GetByIdAsync(id);

        if (director is null)
            throw new NotFoundException(
                "Director was not found.");

        var imageName =
            director.ProfileImageUrl;

        repository.Delete(director);

        await repository.SaveChangesAsync();

        FileManager.DeleteFile(
            imageName,
            GetImageFolderPath());
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
                        BuildPosterUrl(
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
                            BuildPosterUrl(
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

    private string? BuildPosterUrl(
        string? fileName,
        string folder)
    {
        if (string.IsNullOrWhiteSpace(
                fileName))
            return null;

        var relativeUrl =
            $"/images/{folder}/{fileName}";

        var request =
            httpContextAccessor
                .HttpContext?
                .Request;

        if (request is null)
            return relativeUrl;

        return
            $"{request.Scheme}://{request.Host}{relativeUrl}";
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
            "directors");
    }
}
