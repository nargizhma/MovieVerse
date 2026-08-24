using MovieVerse.Dtos.Writers;

namespace MovieVerse.Services.Interfaces;

public interface IWriterService
{
    Task<List<WriterReturnDto>> GetAllAsync();

    Task<WriterReturnDto> GetByIdAsync(Guid id);

    Task CreateAsync(WriterCreateDto dto);

    Task UpdateAsync(Guid id, WriterUpdateDto dto);

    Task DeleteAsync(Guid id);
}