using CurriculoInterativo.Api.DTOs;

namespace CurriculoInterativo.Api.Services.ExperienceService
{
    public interface IExperienceService
    {
        Task<List<ExperienceDto>> GetExperiencesAsync();
    }
}
