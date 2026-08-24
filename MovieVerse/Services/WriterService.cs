using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MovieVerse.Dtos.Writers;
using MovieVerse.Exceptions;
using MovieVerse.Extensions;
using MovieVerse.Models;
using MovieVerse.Repositories.Interfaces;
using MovieVerse.Services.Interfaces;

namespace MovieVerse.Services;

public class WriterService(
    IGenericRepository<Writer> repository,
    IMapper mapper,
    IWebHostEnvironment environment)
    : IWriterService
{
    public async Task<List<WriterReturnDto>> GetAllAsync()
    {
        var writers = await repository.Query()
            .Include(x => x.WriterDetail)
            .AsNoTracking()
            .ToListAsync();

        return mapper.Map<List<WriterReturnDto>>(writers);
    }

    public async Task<WriterReturnDto> GetByIdAsync(Guid id)
    {
        var writer = await repository.Query()
            .Include(x => x.WriterDetail)
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);

        if (writer is null)
            throw new NotFoundException(
                "Writer was not found.");

        return mapper.Map<WriterReturnDto>(writer);
    }

    public async Task CreateAsync(
        WriterCreateDto dto)
    {
        var writer =
            mapper.Map<Writer>(dto);

        if (dto.ProfileImage is not null)
        {
            writer.ProfileImageUrl =
                await dto.ProfileImage.SaveFileAsync(
                    GetImageFolderPath());
        }

        await repository.AddAsync(writer);

        await repository.SaveChangesAsync();
    }

    public async Task UpdateAsync(
        Guid id,
        WriterUpdateDto dto)
    {
        var writer = await repository.Query()
            .Include(x => x.WriterDetail)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (writer is null)
            throw new NotFoundException(
                "Writer was not found.");

        var oldImage =
            writer.ProfileImageUrl;

        mapper.Map(dto, writer);

        if (writer.WriterDetail is null)
        {
            writer.WriterDetail =
                mapper.Map<WriterDetail>(dto);
        }
        else
        {
            mapper.Map(
                dto,
                writer.WriterDetail);
        }

        if (dto.ProfileImage is not null)
        {
            writer.ProfileImageUrl =
                await dto.ProfileImage.SaveFileAsync(
                    GetImageFolderPath());
        }

        repository.Update(writer);

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
        var writer =
            await repository.GetByIdAsync(id);

        if (writer is null)
            throw new NotFoundException(
                "Writer was not found.");

        var imageName =
            writer.ProfileImageUrl;

        repository.Delete(writer);

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
            "writers");
    }
}