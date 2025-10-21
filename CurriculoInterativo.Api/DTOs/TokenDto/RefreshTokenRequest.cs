using System.ComponentModel.DataAnnotations;

namespace CurriculoInterativo.Api.DTOs.TokenDto
{
    public class RefreshTokenRequest
    {
        [Required(ErrorMessage = "Refresh token é obrigatório")]
        public string RefreshToken { get; set; } = string.Empty;
    }
}
