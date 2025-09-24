using System.ComponentModel.DataAnnotations;

namespace CurriculoInterativo.Api.DTOs
{
    public class RegisterDto
    {
        [Required(ErrorMessage = "Nome de usuário é obrigatório")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Nome de usuário deve ter entre 3 e 100 caracteres")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email é obrigatório")]
        [EmailAddress(ErrorMessage = "Email deve ter um formato válido")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Senha é obrigatória")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Senha deve ter entre 6 e 100 caracteres")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Confirmação de senha é obrigatória")]
        [Compare("Password", ErrorMessage = "Senha e confirmação devem ser iguais")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
