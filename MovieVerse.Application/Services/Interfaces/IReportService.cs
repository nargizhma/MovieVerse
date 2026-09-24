using MovieVerse.Dtos.Reports;

namespace MovieVerse.Services.Interfaces;

public interface IReportService
{
    Task<GeneratedReportDto>
        GenerateMyReportAsync(
            Guid userId);

    Task<GeneratedReportDto>
        GeneratePersonReportAsync(
            string personType,
            Guid personId);
}