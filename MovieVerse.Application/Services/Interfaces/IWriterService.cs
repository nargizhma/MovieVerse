using MovieVerse.Dtos.Writers;
using MovieVerse.Requests.Writers;

namespace MovieVerse.Services.Interfaces;

public interface IWriterService
{
    Task<List<WriterReturnDto>> GetAllAsync();

    Task<WriterDetailsDto> GetByIdAsync(Guid id);

    Task CreateAsync(
        WriterCreateRequest request);

    Task UpdateAsync(
        Guid id,
        WriterUpdateRequest request);

    Task DeleteAsync(Guid id);
}