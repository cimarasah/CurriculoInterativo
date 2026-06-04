using System.ComponentModel.DataAnnotations;

namespace CurriculoInterativo.Api.Entities
{
    public class PasswordResetToken
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        [StringLength(500)]
        public string Token { get; set; } = string.Empty;

        [Required]
        public DateTime ExpiresAt { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public bool IsUsed { get; set; } = false;

        public DateTime? UsedAt { get; set; }

        // Navigation property
        public User? User { get; set; }
    }
}

