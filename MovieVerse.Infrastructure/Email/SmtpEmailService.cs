using System.Text.Encodings.Web;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using MovieVerse.Abstractions.Email;

namespace MovieVerse.Infrastructure.Email;

public class SmtpEmailService(
    IOptions<EmailSettings> options)
    : IEmailService
{
    private readonly EmailSettings _settings =
        options.Value;

    public async Task SendEmailConfirmationAsync(
        string email,
        Guid userId,
        string token)
    {
        var confirmationUrl =
            $"{_settings.FrontendBaseUrl.TrimEnd('/')}" +
            $"/verify-email.html" +
            $"?userId={userId}" +
            $"&token={Uri.EscapeDataString(token)}";

        var safeUrl =
            HtmlEncoder.Default.Encode(
                confirmationUrl);

        var message = new MimeMessage();

        message.From.Add(
            new MailboxAddress(
                _settings.FromName,
                _settings.FromEmail));

        message.To.Add(
            MailboxAddress.Parse(email));

        message.Subject =
            "Verify your MovieVerse email";

        message.Body =
            new BodyBuilder
            {
                HtmlBody = $"""
                    <h2>Welcome to MovieVerse</h2>

                    <p>
                        Please verify your email address
                        before logging in.
                    </p>

                    <p>
                        <a href="{safeUrl}">
                            Verify my email
                        </a>
                    </p>

                    <p>
                        If you did not create this account,
                        you can ignore this email.
                    </p>
                    """,

                TextBody =
                    $"Verify your MovieVerse email: {confirmationUrl}"
            }
            .ToMessageBody();

        using var client =
            new SmtpClient();

        await client.ConnectAsync(
            _settings.Host,
            _settings.Port,
            SecureSocketOptions
                .StartTlsWhenAvailable);

        await client.AuthenticateAsync(
            _settings.UserName,
            _settings.Password);

        await client.SendAsync(
            message);

        await client.DisconnectAsync(
            true);
    }
}