using CurriculoInterativo.Api.DTOs.TokenDto;

namespace CurriculoInterativo.Api.Services.GoogleAuthService
{
    public interface IGoogleAuthService
    {
        Task<TokenResponse?> AuthenticateGoogleUserAsync(string googleId, string email, string name);
    }
}

