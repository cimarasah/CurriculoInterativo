
namespace CurriculoInterativo.Api.DTOs.ExperienceDto
{
    public class ExperienceRequest
    { 
        public int Id { get; set; }
        public string Company { get; set; } = string.Empty;
        public string ImgLogo { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Location { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
}

