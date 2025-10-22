using System.ComponentModel.DataAnnotations;

namespace CurriculoInterativo.Api.DTOs.CurriculumDto
{
    public class CurriculumDownloadRequest
    {
        [Required(ErrorMessage = "Nome é obrigatório")]
        [MaxLength(200, ErrorMessage = "Nome deve ter no máximo 200 caracteres")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email é obrigatório")]
        [EmailAddress(ErrorMessage = "Email inválido")]
        [MaxLength(200, ErrorMessage = "Email deve ter no máximo 200 caracteres")]
        public string Email { get; set; } = string.Empty;

        [MaxLength(100, ErrorMessage = "Empresa deve ter no máximo 100 caracteres")]
        public string? Company { get; set; }

        [MaxLength(100, ErrorMessage = "Cargo deve ter no máximo 100 caracteres")]
        public string? Position { get; set; }

        [MaxLength(500, ErrorMessage = "Mensagem deve ter no máximo 500 caracteres")]
        public string? Message { get; set; }
    }
}
