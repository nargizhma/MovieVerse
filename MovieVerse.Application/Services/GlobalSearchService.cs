using MovieVerse.Abstractions.Media;
using MovieVerse.Dtos.Search;
using MovieVerse.Models;
using MovieVerse.Repositories.Interfaces;
using MovieVerse.Services.Interfaces;

namespace MovieVerse.Services;

public class GlobalSearchService(
    IGenericRepository<Movie> movieRepository,
    IGenericRepository<TVShow> tvShowRepository,
    IGenericRepository<Actor> actorRepository,
    IGenericRepository<Director> directorRepository,
    IGenericRepository<Writer> writerRepository,
    IMediaUrlBuilder mediaUrlBuilder)
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

        var movies =
            await movieRepository.FindAllAsync(
                x => x.Title.Contains(search));

        var tvShows =
            await tvShowRepository.FindAllAsync(
                x =>
                    x.Title.Contains(search) ||
                    (
                        x.OriginalTitle != null &&
                        x.OriginalTitle.Contains(search)
                    ));

        var actors =
            await actorRepository.FindAllAsync(
                x => x.FullName.Contains(search));

        var directors =
            await directorRepository.FindAllAsync(
                x => x.FullName.Contains(search));

        var writers =
            await writerRepository.FindAllAsync(
                x => x.FullName.Contains(search));

        return new GlobalSearchResponseDto
        {
            Movies =
                movies
                    .OrderBy(x => x.Title)
                    .Take(take)
                    .Select(x =>
                        new SearchResultItemDto
                        {
                            Id = x.Id,
                            ResultType = "Movie",
                            Title = x.Title,

                            ImageUrl =
                                mediaUrlBuilder.BuildImageUrl(
                                    x.PosterUrl,
                                    "movies"),

                            Subtitle =
                                x.ReleaseDate.Year
                                    .ToString()
                        })
                    .ToList(),

            TVShows =
                tvShows
                    .OrderBy(x => x.Title)
                    .Take(take)
                    .Select(x =>
                        new SearchResultItemDto
                        {
                            Id = x.Id,
                            ResultType = "TVShow",
                            Title = x.Title,

                            ImageUrl =
                                mediaUrlBuilder.BuildImageUrl(
                                    x.PosterUrl,
                                    "tvshows"),

                            Subtitle =
                                x.EndDate.HasValue
                                    ? $"{x.ReleaseDate.Year}-{x.EndDate.Value.Year}"
                                    : $"{x.ReleaseDate.Year}-Present"
                        })
                    .ToList(),

            Actors =
                actors
                    .OrderBy(x => x.FullName)
                    .Take(take)
                    .Select(x =>
                        new SearchResultItemDto
                        {
                            Id = x.Id,
                            ResultType = "Actor",
                            Title = x.FullName,

                            ImageUrl =
                                mediaUrlBuilder.BuildImageUrl(
                                    x.ProfileImageUrl,
                                    "actors"),

                            Subtitle = "Actor"
                        })
                    .ToList(),

            Directors =
                directors
                    .OrderBy(x => x.FullName)
                    .Take(take)
                    .Select(x =>
                        new SearchResultItemDto
                        {
                            Id = x.Id,
                            ResultType = "Director",
                            Title = x.FullName,

                            ImageUrl =
                                mediaUrlBuilder.BuildImageUrl(
                                    x.ProfileImageUrl,
                                    "directors"),

                            Subtitle = "Director"
                        })
                    .ToList(),

            Writers =
                writers
                    .OrderBy(x => x.FullName)
                    .Take(take)
                    .Select(x =>
                        new SearchResultItemDto
                        {
                            Id = x.Id,
                            ResultType = "Writer",
                            Title = x.FullName,

                            ImageUrl =
                                mediaUrlBuilder.BuildImageUrl(
                                    x.ProfileImageUrl,
                                    "writers"),

                            Subtitle = "Writer"
                        })
                    .ToList()
        };
    }
}