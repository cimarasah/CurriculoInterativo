using CurriculoInterativo.Api.DTOs;
using CurriculoInterativo.Api.Services.AuthService;
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
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        /// <summary>
        /// Realiza login do usuário
        /// </summary>
        /// <param name="loginDto">Dados de login</param>
        /// <returns>Token de acesso e informações do usuário</returns>
        [HttpPost("login")]
        public async Task<ActionResult<TokenResponseDto>> Login([FromBody] LoginDto loginDto)
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
        public async Task<ActionResult<TokenResponseDto>> Register([FromBody] RegisterDto registerDto)
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
        public async Task<ActionResult<TokenResponseDto>> RefreshToken([FromBody] RefreshTokenRequest request)
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
        public async Task<ActionResult<UserDto>> GetCurrentUser()
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
