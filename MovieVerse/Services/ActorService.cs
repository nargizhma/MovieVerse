using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MovieVerse.Dtos.Actors;
using MovieVerse.Exceptions;
using MovieVerse.Extensions;
using MovieVerse.Models;
using MovieVerse.Repositories.Interfaces;
using MovieVerse.Services.Interfaces;

namespace MovieVerse.Services;

public class ActorService(
    IGenericRepository<Actor> repository,
    IMapper mapper)
    : IActorService
{
    public async Task<List<ActorReturnDto>> GetAllAsync()
    {
        var actors = await repository.Query()
            .Include(x => x.ActorDetail)
            .AsNoTracking()
            .ToListAsync();

        return mapper.Map<List<ActorReturnDto>>(actors);
    }

    public async Task<ActorReturnDto> GetByIdAsync(Guid id)
    {
        var actor = await repository.Query()
            .Include(x => x.ActorDetail)
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);

        if (actor is null)
            throw new NotFoundException(
                "Actor was not found.");

        return mapper.Map<ActorReturnDto>(actor);
    }

    public async Task CreateAsync(ActorCreateDto dto)
    {
        var actor = mapper.Map<Actor>(dto);

        if (dto.ProfileImage is not null)
        {
            actor.ProfileImageUrl =
                await dto.ProfileImage.SaveFileAsync(
                    Path.Combine(
                        "wwwroot",
                        "images",
                        "actors"));
        }

        await repository.AddAsync(actor);
        await repository.SaveChangesAsync();
    }

    public async Task UpdateAsync(
        Guid id,
        ActorUpdateDto dto)
    {
        var actor = await repository.Query()
            .Include(x => x.ActorDetail)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (actor is null)
            throw new NotFoundException(
                "Actor was not found.");

        mapper.Map(dto, actor);

        if (actor.ActorDetail is null)
        {
            actor.ActorDetail = mapper.Map<ActorDetail>(dto);
        }
        else
        {
            mapper.Map(dto, actor.ActorDetail);
        }

        repository.Update(actor);

        await repository.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var actor = await repository.GetByIdAsync(id);

        if (actor is null)
            throw new NotFoundException(
                "Actor was not found.");

        repository.Delete(actor);

        await repository.SaveChangesAsync();
    }
}