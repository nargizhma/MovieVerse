namespace MovieVerse.Abstractions.Email;

public interface IEmailService
{
    Task SendEmailConfirmationAsync(
        string email,
        Guid userId,
        string token);
}