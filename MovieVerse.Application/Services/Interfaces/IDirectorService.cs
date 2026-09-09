using MovieVerse.Dtos.Directors;
using MovieVerse.Requests.Directors;

namespace MovieVerse.Services.Interfaces;

public interface IDirectorService
{
    Task<List<DirectorReturnDto>> GetAllAsync();

    Task<DirectorDetailsDto> GetByIdAsync(Guid id);

    Task CreateAsync(
        DirectorCreateRequest request);

    Task UpdateAsync(
        Guid id,
        DirectorUpdateRequest request);

    Task DeleteAsync(Guid id);
}