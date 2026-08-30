namespace MovieVerse.Dtos.Profiles;

public class ProfileUpdateDto
{
    public string? DisplayName { get; set; }

    public string? Bio { get; set; }

    public IFormFile? ProfileImage { get; set; }
}
