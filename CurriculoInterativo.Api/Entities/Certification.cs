using CurriculoInterativo.Api.Enums;
using Microsoft.Identity.Client.Platforms.Features.DesktopOs.Kerberos;

namespace CurriculoInterativo.Api.Entities
{
    public class Certification
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Institution { get; set; } = string.Empty;
        public DateTime ObtainedDate { get; set; }
        public string? CertificateUrl { get; set; }
        public string? CredentialCode { get; set; }
        public SkillCategory Category { get; set; }

    }
}

