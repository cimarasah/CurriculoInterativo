namespace CurriculoInterativo.Api.DTOs
{
    public class ListProjectBySkillDto
    {
        public string SkillName { get; set; } = string.Empty;
        public int TotalMonths { get; set; }
        public int Years { get; set; }
        public int Months { get; set; }
        public List<ProjectDto> Projects { get; set; } = new();
    }
}
