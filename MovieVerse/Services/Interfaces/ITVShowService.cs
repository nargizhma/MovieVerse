using MovieVerse.Dtos.Common;
using MovieVerse.Dtos.TVShows;

namespace MovieVerse.Services.Interfaces;

public interface ITVShowService
{
    Task<PagedResultDto<TVShowReturnDto>> GetAllAsync(
        CatalogFilterDto filter);

    Task<TVShowDetailsDto> GetByIdAsync(Guid id);

    Task CreateAsync(TVShowCreateDto dto);

    Task UpdateAsync(
        Guid id,
        TVShowUpdateDto dto);

    Task DeleteAsync(Guid id);
}
