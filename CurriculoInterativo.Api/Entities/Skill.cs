using CurriculoInterativo.Api.Enums;

namespace CurriculoInterativo.Api.Entities
{
    public class Skill
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public SkillCategory Category { get; set; }
        public DateTime StartDate { get; set; }
        public ProficiencyLevel ProficiencyLevel { get; set; } 
        public string Description { get; set; } = string.Empty;
        public bool IsCurrentlyUsed { get; set; }
    }
}

