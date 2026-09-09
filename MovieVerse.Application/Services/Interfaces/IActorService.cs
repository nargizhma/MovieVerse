using MovieVerse.Dtos.Actors;
using MovieVerse.Requests.Actors;

namespace MovieVerse.Services.Interfaces;

public interface IActorService
{
    Task<List<ActorReturnDto>> GetAllAsync();

    Task<ActorDetailsDto> GetByIdAsync(
        Guid id);

    Task CreateAsync(
        ActorCreateRequest request);

    Task UpdateAsync(
        Guid id,
        ActorUpdateRequest request);

    Task DeleteAsync(
        Guid id);
}