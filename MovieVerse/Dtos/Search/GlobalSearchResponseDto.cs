namespace MovieVerse.Dtos.Search;

public class GlobalSearchResponseDto
{
    public List<SearchResultItemDto> Movies { get; set; } = [];

    public List<SearchResultItemDto> TVShows { get; set; } = [];

    public List<SearchResultItemDto> Actors { get; set; } = [];

    public List<SearchResultItemDto> Directors { get; set; } = [];

    public List<SearchResultItemDto> Writers { get; set; } = [];
}