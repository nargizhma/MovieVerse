namespace MovieVerse.Models;

public class UserProfile
{
    public Guid AppUserId { get; set; }

    public string? DisplayName { get; set; }

    public string? Bio { get; set; }

    public string? ProfileImageUrl { get; set; }
}