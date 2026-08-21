using MovieVerse.Dtos.Genres;

namespace MovieVerse.Services.Interfaces;

public interface IGenreService
{
    Task<List<GenreReturnDto>> GetAllAsync();
    Task<GenreReturnDto?> GetByIdAsync(Guid id);
    Task CreateAsync(GenreCreateDto dto);
    Task<bool> UpdateAsync(Guid id, GenreUpdateDto dto);
    Task<bool> DeleteAsync(Guid id);
}