namespace CurriculoInterativo.Api.Models
{
    public class CurriculumModel
    {
        public ContactModel Contact { get; set; } = new();
        public List<ExperienceModel> Experiences { get; set; } = new();
        public List<SkillModel> Skills { get; set; } = new();
        public List<CertificationModel> Certifications { get; set; } = new();
        public List<ProjectModel> Projects { get; set; } = new();
        public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;

    }
}
