namespace CurriculoInterativo.Api.Models
{
    public class CurriculumLeadModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Company { get; set; }
        public string? Position { get; set; }
        public string? Message { get; set; }
        public DateTime RequestedAt { get; set; }
        public int DownloadCount { get; set; }
        public DateTime? LastDownloadAt { get; set; }
    }
}
