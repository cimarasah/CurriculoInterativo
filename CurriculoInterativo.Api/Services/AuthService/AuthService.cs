using AutoMapper;
using CurriculoInterativo.Api.Models;
using CurriculoInterativo.Api.DTOs.TokenDto;
using CurriculoInterativo.Api.Entities;
using CurriculoInterativo.Api.Repositories.UserRepository;
using CurriculoInterativo.Api.Repositories.PasswordResetTokenRepository;
using CurriculoInterativo.Api.Services.TokenService;
using CurriculoInterativo.Api.Services.EmailService;
using Microsoft.AspNetCore.Identity;
using System.Security.Cryptography;
using System.Text;

namespace CurriculoInterativo.Api.Services.AuthService
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly ILogger<AuthService> _logger;
        private readonly IPasswordResetTokenRepository _passwordResetTokenRepository;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;

        public AuthService(
            IUserRepository userRepository,
            ITokenService tokenService,
            IMapper mapper,
            IPasswordHasher<User> passwordHasher,
            ILogger<AuthService> logger,
            IPasswordResetTokenRepository passwordResetTokenRepository,
            IEmailService emailService,
            IConfiguration configuration)
        {
            _userRepository = userRepository;
            _tokenService = tokenService;
            _mapper = mapper;
            _passwordHasher = passwordHasher;
            _logger = logger;
            _passwordResetTokenRepository = passwordResetTokenRepository;
            _emailService = emailService;
            _configuration = configuration;
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

        public async Task<bool> ResetPasswordAsync(ResetPasswordModel resetPasswordDto)
        {
            try
            {
                // Buscar usuário com tracking para poder atualizar
                var user = await _userRepository.GetByEmailForUpdateAsync(resetPasswordDto.Email);

                if (user == null)
                {
                    _logger.LogWarning("Tentativa de reset de senha para email inexistente: {Email}", resetPasswordDto.Email);
                    return false;
                }

                user.PasswordHash = _passwordHasher.HashPassword(user, resetPasswordDto.NewPassword);
                await _userRepository.UpdateAsync(user);

                _logger.LogInformation("Senha resetada com sucesso para usuário: {Email}", resetPasswordDto.Email);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao resetar senha para usuário: {Email}", resetPasswordDto.Email);
                return false;
            }
        }

        public async Task<bool> UpdatePasswordHashAsync(UpdatePasswordHashModel updateHashDto)
        {
            try
            {
                // Buscar usuário com tracking para poder atualizar
                var user = await _userRepository.GetByEmailForUpdateAsync(updateHashDto.Email);

                if (user == null)
                {
                    _logger.LogWarning("Tentativa de atualizar hash para email inexistente: {Email}", updateHashDto.Email);
                    return false;
                }

                user.PasswordHash = updateHashDto.PasswordHash;
                await _userRepository.UpdateAsync(user);

                _logger.LogInformation("Hash de senha atualizado com sucesso para usuário: {Email}", updateHashDto.Email);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar hash de senha para usuário: {Email}", updateHashDto.Email);
                return false;
            }
        }

        public async Task<bool> ForgotPasswordAsync(ForgotPasswordRequest request, string resetUrl)
        {
            try
            {
                var user = await _userRepository.GetByEmailAsync(request.Email);
                
                // Sempre retornar true para não revelar se o email existe ou não (segurança)
                if (user == null || !user.IsActive)
                {
                    _logger.LogWarning("Tentativa de recuperação de senha para email inexistente ou inativo: {Email}", request.Email);
                    return true; // Retorna true mesmo se não existir (segurança)
                }

                // Invalidar tokens anteriores do usuário
                await _passwordResetTokenRepository.InvalidateUserTokensAsync(user.Id);

                // Gerar novo token
                var token = GenerateResetToken();
                var expiresAt = DateTime.UtcNow.AddHours(24); // Token válido por 24 horas

                var resetToken = new PasswordResetToken
                {
                    UserId = user.Id,
                    Token = token,
                    ExpiresAt = expiresAt,
                    IsUsed = false
                };

                await _passwordResetTokenRepository.AddAsync(resetToken);

                // Construir URL completa de reset
                var resetLink = $"{resetUrl}?token={Uri.EscapeDataString(token)}";

                // Enviar email
                var emailBody = BuildResetPasswordEmailBody(user.Username, resetLink);
                var emailSent = await _emailService.SendEmailAsync(
                    user.Email,
                    "Recuperação de Senha - Currículo Interativo",
                    emailBody,
                    isHtml: true);

                if (emailSent)
                {
                    _logger.LogInformation("Email de recuperação de senha enviado para: {Email}", request.Email);
                }
                else
                {
                    _logger.LogWarning("Falha ao enviar email de recuperação de senha para: {Email}", request.Email);
                }

                return true; // Sempre retorna true por segurança
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao processar solicitação de recuperação de senha para: {Email}", request.Email);
                return true; // Retorna true mesmo em caso de erro (segurança)
            }
        }

        public async Task<bool> ResetPasswordWithTokenAsync(ResetPasswordWithTokenRequest request)
        {
            try
            {
                var resetToken = await _passwordResetTokenRepository.GetByTokenAsync(request.Token);

                if (resetToken == null)
                {
                    _logger.LogWarning("Tentativa de reset de senha com token inválido");
                    return false;
                }

                if (resetToken.IsUsed)
                {
                    _logger.LogWarning("Tentativa de reset de senha com token já utilizado");
                    return false;
                }

                if (resetToken.ExpiresAt < DateTime.UtcNow)
                {
                    _logger.LogWarning("Tentativa de reset de senha com token expirado");
                    return false;
                }

                if (resetToken.User == null)
                {
                    _logger.LogError("Token de reset sem usuário associado: {TokenId}", resetToken.Id);
                    return false;
                }

                // Buscar usuário com tracking para atualizar
                var user = await _userRepository.GetByEmailForUpdateAsync(resetToken.User.Email);
                if (user == null)
                {
                    _logger.LogError("Usuário não encontrado para token de reset: {TokenId}", resetToken.Id);
                    return false;
                }

                // Atualizar senha
                user.PasswordHash = _passwordHasher.HashPassword(user, request.NewPassword);
                await _userRepository.UpdateAsync(user);

                // Marcar token como usado
                resetToken.IsUsed = true;
                resetToken.UsedAt = DateTime.UtcNow;
                await _passwordResetTokenRepository.UpdateAsync(resetToken);

                _logger.LogInformation("Senha resetada com sucesso usando token para usuário: {Email}", user.Email);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao resetar senha com token");
                return false;
            }
        }

        private string GenerateResetToken()
        {
            var bytes = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(bytes);
            }
            return Convert.ToBase64String(bytes)
                .Replace("+", "-")
                .Replace("/", "_")
                .Replace("=", "");
        }

        private string BuildResetPasswordEmailBody(string username, string resetLink)
        {
            return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='UTF-8'>
    <style>
        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
        .header {{ background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); color: white; padding: 20px; text-align: center; border-radius: 8px 8px 0 0; }}
        .content {{ background: #f9f9f9; padding: 30px; border-radius: 0 0 8px 8px; }}
        .button {{ display: inline-block; padding: 12px 30px; background: #667eea; color: white; text-decoration: none; border-radius: 5px; margin: 20px 0; }}
        .footer {{ text-align: center; margin-top: 20px; color: #777; font-size: 12px; }}
        .warning {{ background: #fff3cd; border-left: 4px solid #ffc107; padding: 10px; margin: 15px 0; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>Recuperação de Senha</h1>
        </div>
        <div class='content'>
            <p>Olá <strong>{username}</strong>,</p>
            <p>Recebemos uma solicitação para redefinir a senha da sua conta no Currículo Interativo.</p>
            <p>Clique no botão abaixo para criar uma nova senha:</p>
            <p style='text-align: center;'>
                <a href='{resetLink}' class='button'>Redefinir Senha</a>
            </p>
            <p>Ou copie e cole o link abaixo no seu navegador:</p>
            <p style='word-break: break-all; color: #667eea;'>{resetLink}</p>
            <div class='warning'>
                <strong>⚠️ Importante:</strong>
                <ul>
                    <li>Este link é válido por <strong>24 horas</strong></li>
                    <li>Se você não solicitou esta recuperação, ignore este email</li>
                    <li>Não compartilhe este link com ninguém</li>
                </ul>
            </div>
        </div>
        <div class='footer'>
            <p>Este é um email automático, por favor não responda.</p>
            <p>Currículo Interativo - Sistema de Gestão de Currículo</p>
        </div>
    </div>
</body>
</html>";
        }
    }
}
