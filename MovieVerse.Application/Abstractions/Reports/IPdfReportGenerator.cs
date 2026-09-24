using MovieVerse.Dtos.Reports;

namespace MovieVerse.Abstractions.Reports;

public interface IPdfReportGenerator
{
    byte[] GenerateMyMovieVerseReport(
        MyMovieVerseReportDto report);

    byte[] GeneratePersonReport(
        PersonReportDto report);
}