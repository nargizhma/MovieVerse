using MovieVerse.Dtos.Actors;

namespace MovieVerse.Services.Interfaces;

public interface IActorService
{
    Task<List<ActorReturnDto>> GetAllAsync();

    Task<ActorReturnDto> GetByIdAsync(Guid id);

    Task CreateAsync(ActorCreateDto dto);

    Task UpdateAsync(Guid id, ActorUpdateDto dto);

    Task DeleteAsync(Guid id);
}