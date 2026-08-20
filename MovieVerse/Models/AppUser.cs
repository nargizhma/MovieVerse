using Microsoft.AspNetCore.Identity;

namespace MovieVerse.Models
{
    public class AppUser : IdentityUser<Guid>
    {
        public UserProfile? Profile { get; set; }

        public List<Review> MovieReviews { get; set; } = [];
        public List<TVShowReview> TVShowReviews { get; set; } = [];
        public List<EpisodeReview> EpisodeReviews { get; set; } = [];
        public List<WatchlistItem> WatchlistItems { get; set; } = new List<WatchlistItem>();
        public List<WatchHistoryItem> WatchHistoryItems { get; set; } = new List<WatchHistoryItem>();
    }
}
