using System.ComponentModel.DataAnnotations;

namespace CurriculoInterativo.Api.Entities
{
    public class CurriculumGeneration
    {
        [Key]
        public int Id { get; set; }

        public int? UserId { get; set; } // Null se for recrutador não autenticado
        public User? User { get; set; }

        [StringLength(45)]
        public string? IpAddress { get; set; }

        [StringLength(500)]
        public string? UserAgent { get; set; }

        [StringLength(200)]
        public string? CompanyName { get; set; }

        [StringLength(500)]
        public string? JobDescriptionPreview { get; set; } // Primeiros 500 caracteres

        public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;

        [StringLength(50)]
        public string Status { get; set; } = "Success"; // Success, Failed, RateLimited
    }
}
