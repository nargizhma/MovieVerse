using MovieVerse.Dtos.Episodes;

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
        EpisodeCreateDto dto);

    Task UpdateAsync(
        Guid id,
        EpisodeUpdateDto dto);

    Task DeleteAsync(
        Guid id);
}