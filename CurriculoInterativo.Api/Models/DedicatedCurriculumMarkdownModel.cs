namespace CurriculoInterativo.Api.Models
{
    public class DedicatedCurriculumMarkdownModel
    {
        public string Markdown { get; set; } = string.Empty;
        public string? CompanyName { get; set; }
        public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
    }
}
