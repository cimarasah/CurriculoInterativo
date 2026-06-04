using CurriculoInterativo.Api.Models;
using CurriculoInterativo.Api.DTOs.TokenDto;
using CurriculoInterativo.Api.Services.AuthService;
using CurriculoInterativo.Api.Services.GoogleAuthService;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CurriculoInterativo.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IGoogleAuthService _googleAuthService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(
            IAuthService authService, 
            IGoogleAuthService googleAuthService,
            ILogger<AuthController> logger)
        {
            _authService = authService;
            _googleAuthService = googleAuthService;
            _logger = logger;
        }

        /// <summary>
        /// Realiza login do usuário
        /// </summary>
        /// <param name="loginDto">Dados de login</param>
        /// <returns>Token de acesso e informações do usuário</returns>
        [HttpPost("login")]
        public async Task<ActionResult<TokenResponse>> Login([FromBody] LoginModel loginDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = await _authService.LoginAsync(loginDto);

                if (result == null)
                {
                    return Unauthorized(new { message = "Email ou senha inválidos" });
                }

                _logger.LogInformation("Login realizado com sucesso para o usuário: {Email}", loginDto.Email);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro durante o login");
                return StatusCode(500, new { message = "Erro interno do servidor" });
            }
        }

        /// <summary>
        /// Registra um novo usuário (apenas para desenvolvimento - remover em produção)
        /// </summary>
        /// <param name="registerDto">Dados de registro</param>
        /// <returns>Token de acesso e informações do usuário</returns>
        [HttpPost("register")]
        public async Task<ActionResult<TokenResponse>> Register([FromBody] RegisterModel registerDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = await _authService.RegisterAsync(registerDto);

                if (result == null)
                {
                    return BadRequest(new { message = "Usuário ou email já existente" });
                }

                _logger.LogInformation("Usuário registrado com sucesso: {Email}", registerDto.Email);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro durante o registro");
                return StatusCode(500, new { message = "Erro interno do servidor" });
            }
        }

        /// <summary>
        /// Renova o token de acesso usando o refresh token
        /// </summary>
        /// <param name="request">Refresh token</param>
        /// <returns>Novo token de acesso</returns>
        [HttpPost("refresh")]
        public async Task<ActionResult<TokenResponse>> RefreshToken([FromBody] RefreshTokenRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.RefreshToken))
                {
                    return BadRequest(new { message = "Refresh token é obrigatório" });
                }

                var result = await _authService.RefreshTokenAsync(request.RefreshToken);

                if (result == null)
                {
                    return Unauthorized(new { message = "Refresh token inválido" });
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro durante refresh do token");
                return StatusCode(500, new { message = "Erro interno do servidor" });
            }
        }

        /// <summary>
        /// Revoga o refresh token (logout)
        /// </summary>
        /// <param name="request">Refresh token</param>
        /// <returns>Confirmação de logout</returns>
        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout([FromBody] RefreshTokenRequest request)
        {
            try
            {
                if (!string.IsNullOrEmpty(request.RefreshToken))
                {
                    await _authService.RevokeTokenAsync(request.RefreshToken);
                }

                return Ok(new { message = "Logout realizado com sucesso" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro durante o logout");
                return StatusCode(500, new { message = "Erro interno do servidor" });
            }
        }

        /// <summary>
        /// Retorna informações do usuário atual
        /// </summary>
        /// <returns>Dados do usuário logado</returns>
        [HttpGet("me")]
        [Authorize]
        public async Task<ActionResult<UserModel>> GetCurrentUser()
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == null)
                {
                    return Unauthorized(new { message = "Token inválido" });
                }

                var user = await _authService.GetCurrentUserAsync(userId.Value);
                if (user == null)
                {
                    return NotFound(new { message = "Usuário não encontrado" });
                }

                return Ok(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar usuário atual");
                return StatusCode(500, new { message = "Erro interno do servidor" });
            }
        }

        /// <summary>
        /// Reseta a senha de um usuário (apenas para Owner)
        /// </summary>
        /// <param name="resetPasswordDto">Dados para reset de senha</param>
        /// <returns>Confirmação de reset</returns>
        [HttpPost("reset-password")]
        [Authorize(Roles = "Owner")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordModel resetPasswordDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = await _authService.ResetPasswordAsync(resetPasswordDto);

                if (!result)
                {
                    return BadRequest(new { message = "Não foi possível resetar a senha. Verifique se o email está correto." });
                }

                _logger.LogInformation("Senha resetada com sucesso para: {Email}", resetPasswordDto.Email);
                return Ok(new { message = "Senha resetada com sucesso" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao resetar senha");
                return StatusCode(500, new { message = "Erro interno do servidor" });
            }
        }

        /// <summary>
        /// Atualiza o hash de senha diretamente (apenas para Owner - uso administrativo)
        /// ATENÇÃO: Em produção, este endpoint deve ser removido ou protegido adequadamente
        /// </summary>
        /// <param name="updateHashDto">Dados para atualização do hash</param>
        /// <returns>Confirmação de atualização</returns>
        [HttpPost("update-password-hash")]
        [Authorize(Roles = "Owner")]
        public async Task<IActionResult> UpdatePasswordHash([FromBody] UpdatePasswordHashModel updateHashDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = await _authService.UpdatePasswordHashAsync(updateHashDto);

                if (!result)
                {
                    return BadRequest(new { message = "Não foi possível atualizar o hash. Verifique se o email está correto." });
                }

                _logger.LogInformation("Hash de senha atualizado com sucesso para: {Email}", updateHashDto.Email);
                return Ok(new { message = "Hash de senha atualizado com sucesso" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar hash de senha");
                return StatusCode(500, new { message = "Erro interno do servidor" });
            }
        }

        /// <summary>
        /// Inicia o fluxo de autenticação com Google
        /// </summary>
        [HttpGet("google-login")]
        [AllowAnonymous]
        public IActionResult GoogleLogin()
        {
            // Construir URL de callback completa
            var scheme = Request.Scheme;
            var host = Request.Host;
            var callbackUrl = $"{scheme}://{host}/api/auth/google-callback";
            
            var properties = new Microsoft.AspNetCore.Authentication.AuthenticationProperties 
            { 
                RedirectUri = callbackUrl,
                AllowRefresh = true,
                IsPersistent = false
            };
            
            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }

        /// <summary>
        /// Callback do Google OAuth - processa a autenticação
        /// </summary>
        [HttpGet("google-callback")]
        [AllowAnonymous]
        public async Task<IActionResult> GoogleCallback()
        {
            try
            {
                _logger.LogInformation("Google callback recebido. Query: {Query}", Request.QueryString);
                
                // Verificar se há erro na query string
                if (Request.Query.ContainsKey("error"))
                {
                    var error = Request.Query["error"].ToString();
                    var errorDescription = Request.Query.ContainsKey("error_description") 
                        ? Request.Query["error_description"].ToString() 
                        : null;
                    _logger.LogWarning("Erro retornado pelo Google: {Error} - {Description}", error, errorDescription);
                    return Redirect($"/index.html?error=google_auth_failed&details={Uri.EscapeDataString(errorDescription ?? error)}");
                }

                // Autenticar com o esquema do Google (que usa Cookies)
                var result = await HttpContext.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);
                
                if (!result.Succeeded)
                {
                    _logger.LogWarning("Falha na autenticação Google. Succeeded: {Succeeded}, Error: {Error}", 
                        result.Succeeded, result.Failure?.Message);
                    
                    if (result.Failure != null)
                    {
                        _logger.LogError("Exception completa: {Exception}", result.Failure.ToString());
                    }
                    
                    var errorMessage = result.Failure?.Message ?? "Unknown error";
                    return Redirect($"/index.html?error=google_auth_failed&details={Uri.EscapeDataString(errorMessage)}");
                }

                if (result.Principal == null)
                {
                    _logger.LogError("Principal é null após autenticação Google");
                    return Redirect($"/index.html?error=google_auth_failed");
                }

                var claims = result.Principal.Claims.ToList();
                
                // Log todas as claims para debug
                _logger.LogInformation("Claims recebidas do Google ({Count} claims): {Claims}", 
                    claims.Count,
                    string.Join(", ", claims.Select(c => $"{c.Type}={c.Value}")));

                // Tentar múltiplos tipos de claims (compatibilidade com diferentes versões)
                var googleId = claims.FirstOrDefault(c => 
                    c.Type == ClaimTypes.NameIdentifier || 
                    c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier" ||
                    c.Type == "sub")?.Value;
                    
                var email = claims.FirstOrDefault(c => 
                    c.Type == ClaimTypes.Email || 
                    c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress" ||
                    c.Type == "email" ||
                    c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress")?.Value;
                    
                var name = claims.FirstOrDefault(c => 
                    c.Type == ClaimTypes.Name || 
                    c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name" ||
                    c.Type == "name" ||
                    c.Type == "given_name")?.Value
                    ?? email?.Split('@')[0] ?? "Usuário";

                if (string.IsNullOrEmpty(googleId) || string.IsNullOrEmpty(email))
                {
                    _logger.LogError("Informações do Google incompletas. GoogleId: {GoogleId}, Email: {Email}", googleId, email);
                    return Redirect($"/index.html?error=google_info_incomplete");
                }

                var tokenResponse = await _googleAuthService.AuthenticateGoogleUserAsync(googleId, email, name);

                if (tokenResponse == null)
                {
                    _logger.LogError("Falha ao autenticar usuário Google");
                    return Redirect($"/index.html?error=google_auth_failed");
                }

                // Fazer sign out do esquema do Google e Cookies (limpar cookies)
                await HttpContext.SignOutAsync(GoogleDefaults.AuthenticationScheme);
                await HttpContext.SignOutAsync("Cookies");

                // Redirecionar para a página principal com os tokens
                var tokens = System.Text.Json.JsonSerializer.Serialize(tokenResponse);
                var encodedTokens = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(tokens));
                return Redirect($"/index.html?auth=success&tokens={Uri.EscapeDataString(encodedTokens)}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro no callback do Google: {Message}", ex.Message);
                return Redirect($"/index.html?error=google_callback_error&details={Uri.EscapeDataString(ex.Message)}");
            }
        }

        /// <summary>
        /// Obtém o ID do usuário atual do token JWT
        /// </summary>
        /// <returns>ID do usuário ou null se não encontrado</returns>
        private int? GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst("userId")?.Value ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (int.TryParse(userIdClaim, out int userId))
            {
                return userId;
            }

            return null;
        }
    }
}
