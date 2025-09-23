using CurriculoInterativo.Api.Models;

namespace CurriculoInterativo.Api.DTOs
{
    public class ProjectDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Position { get; set; } = string.Empty;
        public List<ResponsibilityDto> Responsibilities { get; set; } = new();
        public List<SkillDto> Skills { get; set; } = new();
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}

