using MovieVerse.Dtos.Common;
using MovieVerse.Dtos.Movies;

namespace MovieVerse.Services.Interfaces;

public interface IMovieService
{
    Task<PagedResultDto<MovieReturnDto>> GetAllAsync(
        CatalogFilterDto filter);

    Task<MovieDetailsDto> GetByIdAsync(Guid id);

    Task CreateAsync(MovieCreateDto dto);

    Task UpdateAsync(
        Guid id,
        MovieUpdateDto dto);

    Task DeleteAsync(Guid id);
}
