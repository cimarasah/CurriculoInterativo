using CurriculoInterativo.Api.Models;

namespace CurriculoInterativo.Api.DTOs
{
    public class ExperienceDto
    {
        public int Id { get; set; }
        public string Company { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Location { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public List<ProjectDto> Projects { get; set; } = new();
    }
}

