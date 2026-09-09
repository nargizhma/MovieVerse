using MovieVerse.Dtos.Common;
using MovieVerse.Dtos.Movies;
using MovieVerse.Requests.Movies;

namespace MovieVerse.Services.Interfaces;

public interface IMovieService
{
    Task<PagedResultDto<MovieReturnDto>> GetAllAsync(
        CatalogFilterDto filter);

    Task<MovieDetailsDto> GetByIdAsync(
        Guid id);

    Task CreateAsync(
        MovieCreateRequest request);

    Task UpdateAsync(
        Guid id,
        MovieUpdateRequest request);

    Task DeleteAsync(Guid id);
}