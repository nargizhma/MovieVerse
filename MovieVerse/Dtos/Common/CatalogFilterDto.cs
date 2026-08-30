namespace MovieVerse.Dtos.Common;

public class CatalogFilterDto
{
    public string? Search { get; set; }

    public Guid? GenreId { get; set; }

    public int? ReleaseYear { get; set; }

    public decimal? MinRating { get; set; }

    public decimal? MaxRating { get; set; }

    public Guid? ActorId { get; set; }

    public Guid? DirectorId { get; set; }

    // title, year, rating
    public string? SortBy { get; set; }

    public bool SortDescending { get; set; } = true;

    public int PageNumber { get; set; } = 1;

    public int PageSize { get; set; } = 12;
}
