using System.ComponentModel.DataAnnotations;

namespace CurriculoInterativo.Api.DTOs
{
    public class SuggestionResponse
    {

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
    }
}

