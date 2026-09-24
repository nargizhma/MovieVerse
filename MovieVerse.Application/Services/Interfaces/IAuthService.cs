using MovieVerse.Dtos.Auth;

namespace MovieVerse.Services.Interfaces
{
    public interface IAuthService
    {
        Task<RegisterResponseDto>
            RegisterAsync(RegisterDto dto);

        Task<AuthResponseDto>
            LoginAsync(LoginDto dto);

        Task ConfirmEmailAsync(
            Guid userId,
            string token);

        Task ResendConfirmationEmailAsync(
            string email);
    }
}
