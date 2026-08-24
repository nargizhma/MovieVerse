using MovieVerse.Dtos.Directors;

namespace MovieVerse.Services.Interfaces;

public interface IDirectorService
{
    Task<List<DirectorReturnDto>> GetAllAsync();

    Task<DirectorReturnDto> GetByIdAsync(Guid id);

    Task CreateAsync(DirectorCreateDto dto);

    Task UpdateAsync(Guid id, DirectorUpdateDto dto);

    Task DeleteAsync(Guid id);
}