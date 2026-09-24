namespace MovieVerse.Dtos.Auth;

public class ConfirmEmailDto
{
    public Guid UserId { get; set; }

    public string Token { get; set; }
        = string.Empty;
}