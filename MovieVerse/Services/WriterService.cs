using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MovieVerse.Dtos.People;
using MovieVerse.Dtos.Writers;
using MovieVerse.Exceptions;
using MovieVerse.Extensions;
using MovieVerse.Models;
using MovieVerse.Repositories.Interfaces;
using MovieVerse.Services.Interfaces;

namespace MovieVerse.Services;

public class WriterService(
    IGenericRepository<Writer> repository,
    IGenericRepository<MovieWriter> movieWriterRepository,
    IGenericRepository<EpisodeWriter> episodeWriterRepository,
    IMapper mapper,
    IWebHostEnvironment environment,
    IHttpContextAccessor httpContextAccessor)
    : IWriterService
{
    public async Task<List<WriterReturnDto>> GetAllAsync()
    {
        var writers = await repository.Query()
            .Include(x => x.WriterDetail)
            .AsNoTracking()
            .ToListAsync();

        return mapper.Map<List<WriterReturnDto>>(writers);
    }

    public async Task<WriterDetailsDto> GetByIdAsync(
        Guid id)
    {
        var writer = await repository.Query()
            .Include(x => x.WriterDetail)
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);

        if (writer is null)
            throw new NotFoundException(
                "Writer was not found.");

        var result =
            mapper.Map<WriterDetailsDto>(
                writer);

        result.Filmography =
            await GetFilmographyAsync(id);

        return result;
    }

    public async Task CreateAsync(
        WriterCreateDto dto)
    {
        var writer =
            mapper.Map<Writer>(dto);

        if (dto.ProfileImage is not null)
        {
            writer.ProfileImageUrl =
                await dto.ProfileImage.SaveFileAsync(
                    GetImageFolderPath());
        }

        await repository.AddAsync(writer);

        await repository.SaveChangesAsync();
    }

    public async Task UpdateAsync(
        Guid id,
        WriterUpdateDto dto)
    {
        var writer = await repository.Query()
            .Include(x => x.WriterDetail)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (writer is null)
            throw new NotFoundException(
                "Writer was not found.");

        var oldImage =
            writer.ProfileImageUrl;

        mapper.Map(dto, writer);

        if (writer.WriterDetail is null)
        {
            writer.WriterDetail =
                mapper.Map<WriterDetail>(dto);
        }
        else
        {
            mapper.Map(
                dto,
                writer.WriterDetail);
        }

        if (dto.ProfileImage is not null)
        {
            writer.ProfileImageUrl =
                await dto.ProfileImage.SaveFileAsync(
                    GetImageFolderPath());
        }

        repository.Update(writer);

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
        var writer =
            await repository.GetByIdAsync(id);

        if (writer is null)
            throw new NotFoundException(
                "Writer was not found.");

        var isUsed =
            await movieWriterRepository.Query()
                .AnyAsync(x =>
                    x.WriterId == id)
            ||
            await episodeWriterRepository.Query()
                .AnyAsync(x =>
                    x.WriterId == id);

        if (isUsed)
            throw new ConflictException(
                "Writer cannot be deleted because they are used in existing titles.");

        var imageName =
            writer.ProfileImageUrl;

        repository.Delete(writer);

        await repository.SaveChangesAsync();

        FileManager.DeleteFile(
            imageName,
            GetImageFolderPath());
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
            "writers");
    }
    private async Task<List<FilmographyItemDto>>
    GetFilmographyAsync(
        Guid writerId)
    {
        var movieCredits =
            await movieWriterRepository.Query()
                .Where(x =>
                    x.WriterId == writerId)
                .Include(x =>
                    x.Movie)
                .AsNoTracking()
                .ToListAsync();

        var episodeCredits =
            await episodeWriterRepository.Query()
                .Where(x =>
                    x.WriterId == writerId)
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
}