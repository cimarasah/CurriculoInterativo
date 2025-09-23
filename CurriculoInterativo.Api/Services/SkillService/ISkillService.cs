using CurriculoInterativo.Api.DTOs;

namespace CurriculoInterativo.Api.Services.SkillService
{
    public interface ISkillService
    {
        Task<List<SkillDto>> GetSkillsAsync();
    }
}
