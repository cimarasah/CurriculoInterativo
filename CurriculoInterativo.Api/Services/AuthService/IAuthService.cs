using CurriculoInterativo.Api.Models;
using CurriculoInterativo.Api.DTOs.TokenDto;

namespace CurriculoInterativo.Api.Services.AuthService
{
    public interface IAuthService
    {
        Task<TokenResponse?> LoginAsync(LoginModel loginDto);
        Task<TokenResponse?> RegisterAsync(RegisterModel registerDto);
        Task<TokenResponse?> RefreshTokenAsync(string refreshToken);
        Task<bool> RevokeTokenAsync(string refreshToken);
        Task<UserModel?> GetCurrentUserAsync(int userId);
        Task<bool> ResetPasswordAsync(ResetPasswordModel resetPasswordDto);
        Task<bool> UpdatePasswordHashAsync(UpdatePasswordHashModel updateHashDto);
        Task<bool> ForgotPasswordAsync(ForgotPasswordRequest request, string resetUrl);
        Task<bool> ResetPasswordWithTokenAsync(ResetPasswordWithTokenRequest request);
    }
}
