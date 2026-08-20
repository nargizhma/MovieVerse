namespace MovieVerse.Dtos.Auth;

public class RegisterDto
{
    public string Email { get; set; } = null!;
    public string UserName { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string? DisplayName { get; set; }
}