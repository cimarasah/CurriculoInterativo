namespace CurriculoInterativo.Api.DTOs
{
    public class CertificationDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Institution { get; set; } = string.Empty;
        public DateTime ObtainedDate { get; set; }
        public string? CertificateUrl { get; set; }
        public string? ImageUrl { get; set; }
        public string Category { get; set; } = string.Empty;
    }
}

