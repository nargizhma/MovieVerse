using MovieVerse.Dtos.Seasons;

namespace MovieVerse.Services.Interfaces;

public interface ISeasonService
{
    Task<List<SeasonReturnDto>>
        GetByTVShowIdAsync(
            Guid tvShowId);

    Task<SeasonReturnDto>
        GetByIdAsync(
            Guid id);

    Task CreateAsync(
        Guid tvShowId,
        SeasonCreateDto dto);

    Task UpdateAsync(
        Guid id,
        SeasonUpdateDto dto);

    Task DeleteAsync(
        Guid id);
}