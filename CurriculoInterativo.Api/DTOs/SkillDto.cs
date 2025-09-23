using CurriculoInterativo.Api.Enums;

namespace CurriculoInterativo.Api.DTOs
{
    public class SkillDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public SkillCategory Category { get; set; }
        public DateTime StartDate { get; set; }
        public ProficiencyLevel ProficiencyLevel { get; set; }
        public string Description { get; set; } = string.Empty;
        public bool IsCurrentlyUsed { get; set; }
        public string ExperienceTime { get; set; } = string.Empty; // Será calculado pela Azure Function
    }
}

