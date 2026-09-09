using AutoMapper;
using MovieVerse.Abstractions.Media;
using MovieVerse.Dtos.People;
using MovieVerse.Dtos.Writers;
using MovieVerse.Exceptions;
using MovieVerse.Models;
using MovieVerse.Repositories.Interfaces;
using MovieVerse.Requests.Writers;
using MovieVerse.Services.Interfaces;

namespace MovieVerse.Services;

public class WriterService(
    IGenericRepository<Writer> repository,
    IGenericRepository<MovieWriter> movieWriterRepository,
    IGenericRepository<EpisodeWriter> episodeWriterRepository,
    IMapper mapper,
    IFileStorage fileStorage,
    IMediaUrlBuilder mediaUrlBuilder)
    : IWriterService
{
    public async Task<List<WriterReturnDto>> GetAllAsync()
    {
        var writers =
            await repository.FindAllAsync(
                x => true,
                false,
                "WriterDetail");

        return mapper.Map<List<WriterReturnDto>>(
            writers);
    }

    public async Task<WriterDetailsDto> GetByIdAsync(
        Guid id)
    {
        var writer =
            await repository.FirstOrDefaultAsync(
                x => x.Id == id,
                false,
                "WriterDetail");

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
        WriterCreateRequest request)
    {
        var writer =
            mapper.Map<Writer>(request);

        string? newImage = null;

        try
        {
            if (request.ProfileImage is not null)
            {
                newImage =
                    await fileStorage.SaveAsync(
                        request.ProfileImage,
                        "writers");

                writer.ProfileImageUrl =
                    newImage;
            }

            await repository.AddAsync(writer);

            await repository.SaveChangesAsync();
        }
        catch
        {
            fileStorage.Delete(
                newImage,
                "writers");

            throw;
        }
    }

    public async Task UpdateAsync(
        Guid id,
        WriterUpdateRequest request)
    {
        var writer =
            await repository.FirstOrDefaultAsync(
                x => x.Id == id,
                true,
                "WriterDetail");

        if (writer is null)
            throw new NotFoundException(
                "Writer was not found.");

        var oldImage =
            writer.ProfileImageUrl;

        mapper.Map(
            request,
            writer);

        if (writer.WriterDetail is null)
        {
            writer.WriterDetail =
                mapper.Map<WriterDetail>(
                    request);
        }
        else
        {
            mapper.Map(
                request,
                writer.WriterDetail);
        }

        string? newImage = null;

        if (request.ProfileImage is not null)
        {
            newImage =
                await fileStorage.SaveAsync(
                    request.ProfileImage,
                    "writers");

            writer.ProfileImageUrl =
                newImage;
        }

        try
        {
            repository.Update(writer);

            await repository.SaveChangesAsync();
        }
        catch
        {
            fileStorage.Delete(
                newImage,
                "writers");

            throw;
        }

        if (newImage is not null)
        {
            fileStorage.Delete(
                oldImage,
                "writers");
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
            await movieWriterRepository.AnyAsync(
                x => x.WriterId == id)
            ||
            await episodeWriterRepository.AnyAsync(
                x => x.WriterId == id);

        if (isUsed)
            throw new ConflictException(
                "Writer cannot be deleted because they are used in existing titles.");

        var imageName =
            writer.ProfileImageUrl;

        repository.Delete(writer);

        await repository.SaveChangesAsync();

        fileStorage.Delete(
            imageName,
            "writers");
    }

    private async Task<List<FilmographyItemDto>>
        GetFilmographyAsync(
            Guid writerId)
    {
        var movieCredits =
            await movieWriterRepository.FindAllAsync(
                x => x.WriterId == writerId,
                false,
                "Movie");

        var episodeCredits =
            await episodeWriterRepository.FindAllAsync(
                x => x.WriterId == writerId,
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