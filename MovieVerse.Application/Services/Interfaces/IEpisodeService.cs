using MovieVerse.Dtos.Episodes;
using MovieVerse.Requests.Episodes;

namespace MovieVerse.Services.Interfaces;

public interface IEpisodeService
{
    Task<List<EpisodeReturnDto>>
        GetBySeasonIdAsync(
            Guid seasonId);

    Task<EpisodeDetailsDto>
        GetByIdAsync(
            Guid id);

    Task CreateAsync(
        Guid seasonId,
        EpisodeCreateRequest request);

    Task UpdateAsync(
        Guid id,
        EpisodeUpdateRequest request);

    Task DeleteAsync(
        Guid id);
}