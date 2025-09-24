using CurriculoInterativo.Api.DTOs;

namespace CurriculoInterativo.Api.Services.AuthService
{
    public interface IAuthService
    {
        Task<TokenResponseDto?> LoginAsync(LoginDto loginDto);
        Task<TokenResponseDto?> RegisterAsync(RegisterDto registerDto);
        Task<TokenResponseDto?> RefreshTokenAsync(string refreshToken);
        Task<bool> RevokeTokenAsync(string refreshToken);
        Task<UserDto?> GetCurrentUserAsync(int userId);
    }
}
