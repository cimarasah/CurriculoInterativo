using CurriculoInterativo.Api.Models;

namespace CurriculoInterativo.Api.DTOs
{
    public class ExperienceLogoDto
    {
        public int Id { get; set; }
        public string Company { get; set; } = string.Empty;
        public string ImgLogo { get; set; } = string.Empty;
    }
}

