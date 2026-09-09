using AutoMapper;
using MovieVerse.Dtos.Genres;
using MovieVerse.Exceptions;
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
        var genres =
            await repository.GetAllAsync();

        return mapper.Map<List<GenreReturnDto>>(
            genres);
    }

    public async Task<GenreReturnDto> GetByIdAsync(
        Guid id)
    {
        var genre =
            await repository.GetByIdAsync(id);

        if (genre is null)
            throw new NotFoundException(
                "Genre was not found.");

        return mapper.Map<GenreReturnDto>(
            genre);
    }

    public async Task CreateAsync(
        GenreCreateDto dto)
    {
        var name = dto.Name.Trim();

        var exists =
            await repository.AnyAsync(x =>
                x.Name.ToLower() ==
                name.ToLower());

        if (exists)
            throw new AlreadyExistsException(
                "Genre with this name already exists.");

        var genre =
            mapper.Map<Genre>(dto);

        genre.Name = name;

        await repository.AddAsync(genre);

        await repository.SaveChangesAsync();
    }

    public async Task UpdateAsync(
        Guid id,
        GenreUpdateDto dto)
    {
        var genre =
            await repository.GetByIdAsync(id);

        if (genre is null)
            throw new NotFoundException(
                "Genre was not found.");

        var name = dto.Name.Trim();

        var exists =
            await repository.AnyAsync(x =>
                x.Id != id &&
                x.Name.ToLower() ==
                name.ToLower());

        if (exists)
            throw new AlreadyExistsException(
                "Genre with this name already exists.");

        mapper.Map(dto, genre);

        genre.Name = name;

        repository.Update(genre);

        await repository.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var genre =
            await repository.GetByIdAsync(id);

        if (genre is null)
            throw new NotFoundException(
                "Genre was not found.");

        var isUsed =
            await repository.AnyAsync(x =>
                x.Id == id &&
                (
                    x.MovieGenres.Any() ||
                    x.TVShowGenres.Any()
                ));

        if (isUsed)
            throw new ConflictException(
                "Genre cannot be deleted because it is used by existing titles.");

        repository.Delete(genre);

        await repository.SaveChangesAsync();
    }
}