using System.ComponentModel.DataAnnotations;

namespace CurriculoInterativo.Api.DTOs.JobApplicationDto
{
    public class JobApplicationRequest
    {
        [Required(ErrorMessage = "A descrição da vaga é obrigatória")]
        [MinLength(50, ErrorMessage = "A descrição da vaga deve ter pelo menos 50 caracteres")]
        public string JobDescription { get; set; } = string.Empty;

        [MaxLength(200)]
        public string? CompanyName { get; set; }
    }
}

