namespace MovieVerse.Dtos.Auth;

public class RegisterResponseDto
{
    public Guid UserId { get; set; }

    public string Email { get; set; }
        = string.Empty;

    public bool RequiresEmailConfirmation
    { get; set; }
}