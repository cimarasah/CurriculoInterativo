using CurriculoInterativo.Api.Models;

namespace CurriculoInterativo.Api.DTOs
{
    public class ProjectDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Position { get; set; } = string.Empty;
        public required ExperienceDto Experience { get; set; }
        public List<ResponsibilityDto> Responsibilities { get; set; } = [];
        public List<SkillDto> Skills { get; set; } = [];
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}

