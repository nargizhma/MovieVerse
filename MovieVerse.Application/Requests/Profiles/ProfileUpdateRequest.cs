using MovieVerse.Abstractions.Media;

namespace MovieVerse.Requests.Profiles;

public class ProfileUpdateRequest
{
    public string? DisplayName { get; set; }

    public string? Bio { get; set; }

    public UploadedFile? ProfileImage { get; set; }
}