using System.ComponentModel.DataAnnotations;

namespace CurriculoInterativo.Api.Entities
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Username { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [StringLength(255)]
        public string Email { get; set; } = string.Empty;

        public string? PasswordHash { get; set; } // Opcional para usuários Google

        [StringLength(255)]
        public string? GoogleId { get; set; } // ID do Google OAuth

        [StringLength(50)]
        public string? Provider { get; set; } // "Google" ou null para login tradicional

        [Required]
        [StringLength(50)]
        public string Role { get; set; } = "User";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? LastLogin { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
