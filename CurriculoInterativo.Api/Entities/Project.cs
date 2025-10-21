namespace CurriculoInterativo.Api.Entities
{
    public class Project
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Position { get; set; } = string.Empty;
        public List<Responsibility> Responsibilities { get; set; } = new();
        public List<Skill> Skills { get; set; } = new();
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int ExperienceId { get; set; }
        public required Experience Experience { get; set; }
    }
}

