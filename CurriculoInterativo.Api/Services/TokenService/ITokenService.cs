using CurriculoInterativo.Api.Models;
using System.Security.Claims;

namespace CurriculoInterativo.Api.Services.TokenService
{
    public interface ITokenService
    {
        string GenerateAccessToken(User user);
        string GenerateRefreshToken();
        ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);
        Task SaveRefreshTokenAsync(int userId, string refreshToken);
        Task<int?> ValidateRefreshTokenAsync(string refreshToken);
        Task RevokeRefreshTokenAsync(string refreshToken);
        Task CleanupExpiredTokensAsync();
    }
}
