using CurriculoInterativo.Api.Enums;

namespace CurriculoInterativo.Api.Models
{
    public class Certification
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Institution { get; set; } = string.Empty;
        public DateTime ObtainedDate { get; set; }
        public string? CertificateUrl { get; set; }
        public string? ImageUrl { get; set; }
        public SkillCategory Category { get; set; }

    }
}

