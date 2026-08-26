using MovieVerse.Dtos.Movies;

namespace MovieVerse.Services.Interfaces;

public interface IMovieService
{
    Task<List<MovieReturnDto>> GetAllAsync();

    Task<MovieDetailsDto> GetByIdAsync(Guid id);

    Task CreateAsync(MovieCreateDto dto);

    Task UpdateAsync(
        Guid id,
        MovieUpdateDto dto);

    Task DeleteAsync(Guid id);
}