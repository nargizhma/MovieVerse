namespace MovieVerse.Dtos.Admin;

public class AdminUserReturnDto
{
    public Guid Id { get; set; }

    public string UserName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string? DisplayName { get; set; }

    public List<string> Roles { get; set; } = [];

    public int ReviewCount { get; set; }

    public int WatchlistCount { get; set; }

    public int WatchHistoryCount { get; set; }
}
