using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MovieVerse.Dtos.Directors;
using MovieVerse.Exceptions;
using MovieVerse.Extensions;
using MovieVerse.Models;
using MovieVerse.Repositories.Interfaces;
using MovieVerse.Services.Interfaces;

namespace MovieVerse.Services;

public class DirectorService(
    IGenericRepository<Director> repository,
    IMapper mapper,
    IWebHostEnvironment environment)
    : IDirectorService
{
    public async Task<List<DirectorReturnDto>> GetAllAsync()
    {
        var directors = await repository.Query()
            .Include(x => x.DirectorDetail)
            .AsNoTracking()
            .ToListAsync();

        return mapper.Map<List<DirectorReturnDto>>(directors);
    }

    public async Task<DirectorReturnDto> GetByIdAsync(Guid id)
    {
        var director = await repository.Query()
            .Include(x => x.DirectorDetail)
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);

        if (director is null)
            throw new NotFoundException(
                "Director was not found.");

        return mapper.Map<DirectorReturnDto>(director);
    }

    public async Task CreateAsync(
        DirectorCreateDto dto)
    {
        var director =
            mapper.Map<Director>(dto);

        if (dto.ProfileImage is not null)
        {
            director.ProfileImageUrl =
                await dto.ProfileImage.SaveFileAsync(
                    GetImageFolderPath());
        }

        await repository.AddAsync(director);

        await repository.SaveChangesAsync();
    }

    public async Task UpdateAsync(
        Guid id,
        DirectorUpdateDto dto)
    {
        var director = await repository.Query()
            .Include(x => x.DirectorDetail)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (director is null)
            throw new NotFoundException(
                "Director was not found.");

        var oldImage =
            director.ProfileImageUrl;

        mapper.Map(dto, director);

        if (director.DirectorDetail is null)
        {
            director.DirectorDetail =
                mapper.Map<DirectorDetail>(dto);
        }
        else
        {
            mapper.Map(
                dto,
                director.DirectorDetail);
        }

        if (dto.ProfileImage is not null)
        {
            director.ProfileImageUrl =
                await dto.ProfileImage.SaveFileAsync(
                    GetImageFolderPath());
        }

        repository.Update(director);

        await repository.SaveChangesAsync();

        if (dto.ProfileImage is not null)
        {
            FileManager.DeleteFile(
                oldImage,
                GetImageFolderPath());
        }
    }

    public async Task DeleteAsync(Guid id)
    {
        var director =
            await repository.GetByIdAsync(id);

        if (director is null)
            throw new NotFoundException(
                "Director was not found.");

        var imageName =
            director.ProfileImageUrl;

        repository.Delete(director);

        await repository.SaveChangesAsync();

        FileManager.DeleteFile(
            imageName,
            GetImageFolderPath());
    }

    private string GetImageFolderPath()
    {
        var webRootPath =
            environment.WebRootPath
            ?? Path.Combine(
                environment.ContentRootPath,
                "wwwroot");

        return Path.Combine(
            webRootPath,
            "images",
            "directors");
    }
}