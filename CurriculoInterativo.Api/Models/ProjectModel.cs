
namespace CurriculoInterativo.Api.Models
{
    public class ProjectModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Position { get; set; } = string.Empty;
        public required ExperienceModel Experience { get; set; }
        public List<ResponsibilityModel> Responsibilities { get; set; } = [];
        public List<SkillModel> Skills { get; set; } = [];
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}

