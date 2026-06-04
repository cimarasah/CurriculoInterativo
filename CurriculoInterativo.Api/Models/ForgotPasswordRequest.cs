using System.ComponentModel.DataAnnotations;

namespace CurriculoInterativo.Api.Models
{
    public class ForgotPasswordRequest
    {
        [Required(ErrorMessage = "Email é obrigatório")]
        [EmailAddress(ErrorMessage = "Email deve ter um formato válido")]
        public string Email { get; set; } = string.Empty;
    }
}

