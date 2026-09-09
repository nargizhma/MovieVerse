using MovieVerse.Dtos.Common;
using MovieVerse.Dtos.TVShows;
using MovieVerse.Requests.TVShows;

namespace MovieVerse.Services.Interfaces;

public interface ITVShowService
{
    Task<PagedResultDto<TVShowReturnDto>> GetAllAsync(
        CatalogFilterDto filter);

    Task<TVShowDetailsDto> GetByIdAsync(
        Guid id);

    Task CreateAsync(
        TVShowCreateRequest request);

    Task UpdateAsync(
        Guid id,
        TVShowUpdateRequest request);

    Task DeleteAsync(Guid id);
}