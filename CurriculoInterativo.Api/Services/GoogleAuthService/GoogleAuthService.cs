using CurriculoInterativo.Api.DTOs.TokenDto;
using CurriculoInterativo.Api.Entities;
using CurriculoInterativo.Api.Repositories.UserRepository;
using CurriculoInterativo.Api.Services.TokenService;
using Microsoft.Extensions.Logging;

namespace CurriculoInterativo.Api.Services.GoogleAuthService
{
    public class GoogleAuthService : IGoogleAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly ITokenService _tokenService;
        private readonly ILogger<GoogleAuthService> _logger;

        public GoogleAuthService(
            IUserRepository userRepository,
            ITokenService tokenService,
            ILogger<GoogleAuthService> logger)
        {
            _userRepository = userRepository;
            _tokenService = tokenService;
            _logger = logger;
        }

        public async Task<TokenResponse?> AuthenticateGoogleUserAsync(string googleId, string email, string name)
        {
            try
            {
                // Buscar usuário por GoogleId ou Email
                var user = await _userRepository.GetByGoogleIdAsync(googleId) 
                    ?? await _userRepository.GetByEmailAsync(email);

                if (user == null)
                {
                    // Criar novo usuário
                    user = new User
                    {
                        Username = name,
                        Email = email,
                        GoogleId = googleId,
                        Provider = "Google",
                        Role = "User",
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow,
                        LastLogin = DateTime.UtcNow
                    };

                    await _userRepository.AddAsync(user);
                    _logger.LogInformation("Novo usuário criado via Google: {Email}", email);
                }
                else
                {
                    // Atualizar usuário existente se necessário
                    if (string.IsNullOrEmpty(user.GoogleId))
                    {
                        user.GoogleId = googleId;
                        user.Provider = "Google";
                    }

                    // Atualizar último login
                    user.LastLogin = DateTime.UtcNow;
                    await _userRepository.UpdateAsync(user);
                    _logger.LogInformation("Usuário autenticado via Google: {Email}", email);
                }

                // Gerar tokens
                var token = _tokenService.GenerateAccessToken(user);
                var refreshToken = _tokenService.GenerateRefreshToken();
                await _tokenService.SaveRefreshTokenAsync(user.Id, refreshToken);

                return new TokenResponse
                {
                    Token = token,
                    RefreshToken = refreshToken,
                    ExpiresAt = DateTime.UtcNow.AddHours(1),
                    Username = user.Username,
                    Email = user.Email,
                    Role = user.Role
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao autenticar usuário Google: {Email}", email);
                return null;
            }
        }
    }
}

