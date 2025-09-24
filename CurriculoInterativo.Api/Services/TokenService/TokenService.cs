using CurriculoInterativo.Api.Models;
using CurriculoInterativo.Api.Repositories.RefreshTokenRepository;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace CurriculoInterativo.Api.Services.TokenService
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _configuration;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly ILogger<TokenService> _logger;

        public TokenService(
            IConfiguration configuration,
            IRefreshTokenRepository refreshTokenRepository,
            ILogger<TokenService> logger)
        {
            _configuration = configuration;
            _refreshTokenRepository = refreshTokenRepository;
            _logger = logger;
        }

        public string GenerateAccessToken(User user)
        {
            var key = Encoding.UTF8.GetBytes(_configuration["JwtSettings:SecretKey"]
                ?? throw new InvalidOperationException("JWT SecretKey não configurada"));

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role, user.Role),
                    new Claim("userId", user.Id.ToString())
                }),
                Expires = DateTime.UtcNow.AddHours(1),
                Issuer = _configuration["JwtSettings:Issuer"],
                Audience = _configuration["JwtSettings:Audience"],
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            _logger.LogInformation("Token de acesso gerado para usuário: {UserId}", user.Id);

            return tokenHandler.WriteToken(token);
        }

        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        public ClaimsPrincipal? GetPrincipalFromExpiredToken(string token)
        {
            try
            {
                var key = Encoding.UTF8.GetBytes(_configuration["JwtSettings:SecretKey"]
                    ?? throw new InvalidOperationException("JWT SecretKey não configurada"));

                var tokenHandler = new JwtSecurityTokenHandler();
                var validationParameters = new TokenValidationParameters
                {
                    ValidateAudience = true,
                    ValidateIssuer = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateLifetime = false, // Não validar expiração para refresh
                    ValidIssuer = _configuration["JwtSettings:Issuer"],
                    ValidAudience = _configuration["JwtSettings:Audience"]
                };

                var principal = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);

                if (validatedToken is not JwtSecurityToken jwtToken ||
                    !jwtToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                {
                    _logger.LogWarning("Token inválido recebido");
                    return null;
                }

                return principal;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao validar token expirado");
                return null;
            }
        }

        public async Task SaveRefreshTokenAsync(int userId, string refreshToken)
        {
            try
            {
                var tokenEntity = new RefreshToken
                {
                    Token = refreshToken,
                    UserId = userId,
                    ExpiryDate = DateTime.UtcNow.AddDays(7), // Refresh token expira em 7 dias
                    IsRevoked = false,
                    CreatedAt = DateTime.UtcNow
                };

                await _refreshTokenRepository.AddAsync(tokenEntity);
                _logger.LogInformation("Refresh token salvo para usuário: {UserId}", userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao salvar refresh token para usuário: {UserId}", userId);
                throw;
            }
        }

        public async Task<int?> ValidateRefreshTokenAsync(string refreshToken)
        {
            try
            {
                var tokenEntity = await _refreshTokenRepository.GetByTokenAsync(refreshToken);

                if (tokenEntity == null ||
                    tokenEntity.IsRevoked ||
                    tokenEntity.ExpiryDate <= DateTime.UtcNow)
                {
                    _logger.LogWarning("Refresh token inválido, revogado ou expirado");
                    return null;
                }

                _logger.LogInformation("Refresh token validado com sucesso para usuário: {UserId}", tokenEntity.UserId);
                return tokenEntity.UserId;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao validar refresh token");
                return null;
            }
        }

        public async Task RevokeRefreshTokenAsync(string refreshToken)
        {
            try
            {
                await _refreshTokenRepository.RevokeTokenAsync(refreshToken);
                _logger.LogInformation("Refresh token revogado");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao revogar refresh token");
                throw;
            }
        }

        public async Task CleanupExpiredTokensAsync()
        {
            try
            {
                await _refreshTokenRepository.DeleteExpiredTokensAsync();
                _logger.LogInformation("Tokens expirados removidos com sucesso");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao limpar tokens expirados");
                throw;
            }
        }
    }
}
