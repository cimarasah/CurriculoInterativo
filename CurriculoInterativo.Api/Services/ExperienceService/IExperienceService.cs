using CurriculoInterativo.Api.DTOs.ExperienceDto;

namespace CurriculoInterativo.Api.Services.ExperienceService
{
    public interface IExperienceService
    {
        Task<List<ExperienceResponse>> GetExperiencesMiniAsync();
    }
}
