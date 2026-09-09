using MovieVerse.Dtos.Genres;

namespace MovieVerse.Services.Interfaces;

public interface IGenreService
{
    Task<List<GenreReturnDto>> GetAllAsync();
    Task<GenreReturnDto> GetByIdAsync(Guid id);
    Task CreateAsync(GenreCreateDto dto);
    Task UpdateAsync(Guid id, GenreUpdateDto dto);
    Task DeleteAsync(Guid id);
}