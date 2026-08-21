using AutoMapper;
using MovieVerse.Dtos.Genres;
using MovieVerse.Models;
using MovieVerse.Repositories.Interfaces;
using MovieVerse.Services.Interfaces;

namespace MovieVerse.Services;

public class GenreService(
    IGenericRepository<Genre> repository,
    IMapper mapper)
    : IGenreService
{
    public async Task<List<GenreReturnDto>> GetAllAsync()
    {
        var genres = await repository.GetAllAsync();

        return mapper.Map<List<GenreReturnDto>>(genres);
    }

    public async Task<GenreReturnDto?> GetByIdAsync(Guid id)
    {
        var genre = await repository.GetByIdAsync(id);

        if (genre is null)
            return null;

        return mapper.Map<GenreReturnDto>(genre);
    }

    public async Task CreateAsync(GenreCreateDto dto)
    {
        var genre = mapper.Map<Genre>(dto);

        await repository.AddAsync(genre);
        await repository.SaveChangesAsync();
    }

    public async Task<bool> UpdateAsync(Guid id, GenreUpdateDto dto)
    {
        var genre = await repository.GetByIdAsync(id);

        if (genre is null)
            return false;

        mapper.Map(dto, genre);

        repository.Update(genre);
        await repository.SaveChangesAsync();

        return true;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var genre = await repository.GetByIdAsync(id);

        if (genre is null)
            return false;

        repository.Delete(genre);
        await repository.SaveChangesAsync();

        return true;
    }
}