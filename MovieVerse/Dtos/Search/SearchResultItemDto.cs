namespace MovieVerse.Dtos.Search;

public class SearchResultItemDto
{
    public Guid Id { get; set; }

    public string ResultType { get; set; } = null!;

    public string Title { get; set; } = null!;

    public string? ImageUrl { get; set; }

    public string? Subtitle { get; set; }
}