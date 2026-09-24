using MovieVerse.Dtos.Profiles;
using MovieVerse.Dtos.UserLibrary;

namespace MovieVerse.Dtos.Reports;

public class MyMovieVerseReportDto
{
    public MyProfileReturnDto Profile { get; set; } = null!;

    public List<ProfileActivityItemDto> Reviews { get; set; } = [];

    public List<LibraryItemReturnDto> Watchlist { get; set; } = [];

    public List<LibraryItemReturnDto> WatchHistory { get; set; } = [];

    public DateTime GeneratedAt { get; set; }
}