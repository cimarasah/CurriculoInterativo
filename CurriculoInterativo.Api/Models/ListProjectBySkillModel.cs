namespace CurriculoInterativo.Api.Models
{
    public class ListProjectBySkillModel
    {
        public string SkillName { get; set; } = string.Empty;
        public int TotalMonths { get; set; }
        public int Years { get; set; }
        public int Months { get; set; }
        public List<ProjectModel> Projects { get; set; } = new();
    }
}
