using System.ComponentModel.DataAnnotations;

namespace CurriculoInterativo.Api.Entities
{
    public class Suggestion
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [MaxLength(200)]
        public string Email { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? Company { get; set; }

        [MaxLength(100)]
        public string? Position { get; set; }

        [MaxLength(500)]
        public string? Message { get; set; }

        public DateTime RequestedAt { get; set; } = DateTime.UtcNow;

        [MaxLength(45)]
        public string? IpAddress { get; set; }

        [MaxLength(500)]
        public string? UserAgent { get; set; }

    }
}
