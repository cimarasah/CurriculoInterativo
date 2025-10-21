using AutoMapper;
using CurriculoInterativo.Api.Models;
using CurriculoInterativo.Api.DTOs.TokenDto;
using CurriculoInterativo.Api.Entities;
using CurriculoInterativo.Api.Repositories.UserRepository;
using CurriculoInterativo.Api.Services.TokenService;
using Microsoft.AspNetCore.Identity;

namespace CurriculoInterativo.Api.Services.AuthService
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly ILogger<AuthService> _logger;

        public AuthService(
            IUserRepository userRepository,
            ITokenService tokenService,
            IMapper mapper,
            IPasswordHasher<User> passwordHasher,
            ILogger<AuthService> logger)
        {
            _userRepository = userRepository;
            _tokenService = tokenService;
            _mapper = mapper;
            _passwordHasher = passwordHasher;
            _logger = logger;
        }

        public async Task<TokenResponse?> LoginAsync(LoginModel loginDto)
        {
            try
            {
                var user = await _userRepository.GetByEmailAsync(loginDto.Email);
                if (user == null || !user.IsActive)
                {
                    _logger.LogWarning("Tentativa de login com email inexistente ou usuário inativo: {Email}", loginDto.Email);
                    return null;
                }

                var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, loginDto.Password);
                if (result == PasswordVerificationResult.Failed)
                {
                    _logger.LogWarning("Tentativa de login com senha incorreta para usuário: {Email}", loginDto.Email);
                    return null;
                }

                // Atualizar último login
                user.LastLogin = DateTime.UtcNow;
                await _userRepository.UpdateAsync(user);

                var token = _tokenService.GenerateAccessToken(user);
                var refreshToken = _tokenService.GenerateRefreshToken();

                await _tokenService.SaveRefreshTokenAsync(user.Id, refreshToken);

                _logger.LogInformation("Login realizado com sucesso para usuário: {Email}", loginDto.Email);

                return new TokenResponse
                {
                    Token = token,
                    RefreshToken = refreshToken,
                    ExpiresAt = DateTime.UtcNow.AddHours(1), // Token expira em 1 hora
                    Username = user.Username,
                    Email = user.Email,
                    Role = user.Role
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro durante o login para usuário: {Email}", loginDto.Email);
                return null;
            }
        }

        public async Task<TokenResponse?> RegisterAsync(RegisterModel registerDto)
        {
            try
            {
                // Verificar se o usuário já existe
                if (await _userRepository.GetByEmailAsync(registerDto.Email) != null)
                {
                    _logger.LogWarning("Tentativa de registro com email já existente: {Email}", registerDto.Email);
                    return null;
                }

                if (await _userRepository.GetByUsernameAsync(registerDto.Username) != null)
                {
                    _logger.LogWarning("Tentativa de registro com username já existente: {Username}", registerDto.Username);
                    return null;
                }

                var user = new User
                {
                    Username = registerDto.Username,
                    Email = registerDto.Email,
                    Role = "Owner", // Primeiro usuário será o proprietário
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true
                };

                user.PasswordHash = _passwordHasher.HashPassword(user, registerDto.Password);

                await _userRepository.AddAsync(user);

                var token = _tokenService.GenerateAccessToken(user);
                var refreshToken = _tokenService.GenerateRefreshToken();

                await _tokenService.SaveRefreshTokenAsync(user.Id, refreshToken);

                _logger.LogInformation("Usuário registrado com sucesso: {Email}", registerDto.Email);

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
                _logger.LogError(ex, "Erro durante o registro do usuário: {Email}", registerDto.Email);
                return null;
            }
        }

        public async Task<TokenResponse?> RefreshTokenAsync(string refreshToken)
        {
            try
            {
                var userId = await _tokenService.ValidateRefreshTokenAsync(refreshToken);
                if (userId == null)
                {
                    _logger.LogWarning("Tentativa de refresh com token inválido");
                    return null;
                }

                var user = await _userRepository.GetByIdAsync(userId.Value);
                if (user == null || !user.IsActive)
                {
                    _logger.LogWarning("Tentativa de refresh para usuário inexistente ou inativo: {UserId}", userId);
                    return null;
                }

                var newToken = _tokenService.GenerateAccessToken(user);
                var newRefreshToken = _tokenService.GenerateRefreshToken();

                await _tokenService.RevokeRefreshTokenAsync(refreshToken);
                await _tokenService.SaveRefreshTokenAsync(user.Id, newRefreshToken);

                _logger.LogInformation("Token renovado com sucesso para usuário: {UserId}", userId);

                return new TokenResponse
                {
                    Token = newToken,
                    RefreshToken = newRefreshToken,
                    ExpiresAt = DateTime.UtcNow.AddHours(1),
                    Username = user.Username,
                    Email = user.Email,
                    Role = user.Role
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro durante o refresh token");
                return null;
            }
        }

        public async Task<bool> RevokeTokenAsync(string refreshToken)
        {
            try
            {
                await _tokenService.RevokeRefreshTokenAsync(refreshToken);
                _logger.LogInformation("Token revogado com sucesso");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao revogar token");
                return false;
            }
        }

        public async Task<UserModel?> GetCurrentUserAsync(int userId)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(userId);
                return user == null ? null : _mapper.Map<UserModel>(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar usuário atual: {UserId}", userId);
                return null;
            }
        }
    }
}
