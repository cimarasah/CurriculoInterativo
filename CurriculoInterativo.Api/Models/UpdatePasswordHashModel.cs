using System.ComponentModel.DataAnnotations;

namespace CurriculoInterativo.Api.Models
{
    public class UpdatePasswordHashModel
    {
        [Required(ErrorMessage = "Email é obrigatório")]
        [EmailAddress(ErrorMessage = "Email deve ter um formato válido")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Hash da senha é obrigatório")]
        public string PasswordHash { get; set; } = string.Empty;
    }
}

