namespace MovieVerse.Dtos.Profiles;

public class UserProfileReturnDto
{
    public Guid UserId { get; set; }

    public string UserName { get; set; } = null!;

    public string? DisplayName { get; set; }

    public string? Bio { get; set; }

    public string? ProfileImageUrl { get; set; }

    public int MovieReviewCount { get; set; }

    public int TVShowReviewCount { get; set; }

    public int EpisodeReviewCount { get; set; }

    public int TotalReviewCount =>
        MovieReviewCount +
        TVShowReviewCount +
        EpisodeReviewCount;

    public int WatchlistCount { get; set; }

    public int WatchHistoryCount { get; set; }
}
