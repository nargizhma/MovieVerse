namespace MovieVerse.Dtos.Admin;

public class AdminDashboardStatsDto
{
    public int UserCount { get; set; }

    public int AdminCount { get; set; }

    public int SuperAdminCount { get; set; }

    public int MovieCount { get; set; }

    public int TVShowCount { get; set; }

    public int SeasonCount { get; set; }

    public int EpisodeCount { get; set; }

    public int ActorCount { get; set; }

    public int DirectorCount { get; set; }

    public int WriterCount { get; set; }

    public int GenreCount { get; set; }

    public int MovieReviewCount { get; set; }

    public int TVShowReviewCount { get; set; }

    public int EpisodeReviewCount { get; set; }

    public int TotalReviewCount =>
        MovieReviewCount +
        TVShowReviewCount +
        EpisodeReviewCount;

    public int WatchlistItemCount { get; set; }

    public int WatchHistoryItemCount { get; set; }
}
