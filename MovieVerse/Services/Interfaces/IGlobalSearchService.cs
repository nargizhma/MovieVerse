using MovieVerse.Dtos.Search;

namespace MovieVerse.Services.Interfaces;

public interface IGlobalSearchService
{
    Task<GlobalSearchResponseDto> SearchAsync(
        string query,
        int limit);
}