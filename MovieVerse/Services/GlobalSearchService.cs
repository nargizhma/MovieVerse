using Microsoft.EntityFrameworkCore;
using MovieVerse.Data;
using MovieVerse.Dtos.Search;
using MovieVerse.Services.Interfaces;

namespace MovieVerse.Services;

public class GlobalSearchService(
    AppDbContext dbContext,
    IHttpContextAccessor httpContextAccessor)
    : IGlobalSearchService
{
    public async Task<GlobalSearchResponseDto> SearchAsync(
        string query,
        int limit)
    {
        var search =
            query.Trim();

        var take =
            Math.Clamp(
                limit,
                1,
                10);

        var pattern =
            $"%{search}%";

        var movies =
            await dbContext.Movies
                .Where(x =>
                    EF.Functions.Like(
                        x.Title,
                        pattern))
                .OrderBy(x =>
                    x.Title)
                .Take(take)
                .Select(x =>
                    new
                    {
                        x.Id,
                        x.Title,
                        x.PosterUrl,
                        x.ReleaseDate
                    })
                .AsNoTracking()
                .ToListAsync();

        var tvShows =
            await dbContext.TVShows
                .Where(x =>
                    EF.Functions.Like(
                        x.Title,
                        pattern)
                    ||
                    (x.OriginalTitle != null &&
                     EF.Functions.Like(
                         x.OriginalTitle,
                         pattern)))
                .OrderBy(x =>
                    x.Title)
                .Take(take)
                .Select(x =>
                    new
                    {
                        x.Id,
                        x.Title,
                        x.PosterUrl,
                        x.ReleaseDate,
                        x.EndDate
                    })
                .AsNoTracking()
                .ToListAsync();

        var actors =
            await dbContext.Actors
                .Where(x =>
                    EF.Functions.Like(
                        x.FullName,
                        pattern))
                .OrderBy(x =>
                    x.FullName)
                .Take(take)
                .Select(x =>
                    new
                    {
                        x.Id,
                        x.FullName,
                        x.ProfileImageUrl
                    })
                .AsNoTracking()
                .ToListAsync();

        var directors =
            await dbContext.Directors
                .Where(x =>
                    EF.Functions.Like(
                        x.FullName,
                        pattern))
                .OrderBy(x =>
                    x.FullName)
                .Take(take)
                .Select(x =>
                    new
                    {
                        x.Id,
                        x.FullName,
                        x.ProfileImageUrl
                    })
                .AsNoTracking()
                .ToListAsync();

        var writers =
            await dbContext.Writers
                .Where(x =>
                    EF.Functions.Like(
                        x.FullName,
                        pattern))
                .OrderBy(x =>
                    x.FullName)
                .Take(take)
                .Select(x =>
                    new
                    {
                        x.Id,
                        x.FullName,
                        x.ProfileImageUrl
                    })
                .AsNoTracking()
                .ToListAsync();

        return new GlobalSearchResponseDto
        {
            Movies =
                movies.Select(x =>
                    new SearchResultItemDto
                    {
                        Id =
                            x.Id,

                        ResultType =
                            "Movie",

                        Title =
                            x.Title,

                        ImageUrl =
                            BuildImageUrl(
                                x.PosterUrl,
                                "movies"),

                        Subtitle =
                            x.ReleaseDate.Year
                                .ToString()
                    })
                    .ToList(),

            TVShows =
                tvShows.Select(x =>
                    new SearchResultItemDto
                    {
                        Id =
                            x.Id,

                        ResultType =
                            "TVShow",

                        Title =
                            x.Title,

                        ImageUrl =
                            BuildImageUrl(
                                x.PosterUrl,
                                "tvshows"),

                        Subtitle =
                            x.EndDate.HasValue
                                ? $"{x.ReleaseDate.Year}-{x.EndDate.Value.Year}"
                                : $"{x.ReleaseDate.Year}-Present"
                    })
                    .ToList(),

            Actors =
                actors.Select(x =>
                    new SearchResultItemDto
                    {
                        Id =
                            x.Id,

                        ResultType =
                            "Actor",

                        Title =
                            x.FullName,

                        ImageUrl =
                            BuildImageUrl(
                                x.ProfileImageUrl,
                                "actors"),

                        Subtitle =
                            "Actor"
                    })
                    .ToList(),

            Directors =
                directors.Select(x =>
                    new SearchResultItemDto
                    {
                        Id =
                            x.Id,

                        ResultType =
                            "Director",

                        Title =
                            x.FullName,

                        ImageUrl =
                            BuildImageUrl(
                                x.ProfileImageUrl,
                                "directors"),

                        Subtitle =
                            "Director"
                    })
                    .ToList(),

            Writers =
                writers.Select(x =>
                    new SearchResultItemDto
                    {
                        Id =
                            x.Id,

                        ResultType =
                            "Writer",

                        Title =
                            x.FullName,

                        ImageUrl =
                            BuildImageUrl(
                                x.ProfileImageUrl,
                                "writers"),

                        Subtitle =
                            "Writer"
                    })
                    .ToList()
        };
    }

    private string? BuildImageUrl(
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