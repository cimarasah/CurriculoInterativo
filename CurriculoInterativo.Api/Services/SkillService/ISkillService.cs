using CurriculoInterativo.Api.Models;

namespace CurriculoInterativo.Api.Services.SkillService
{
    public interface ISkillService
    {
        Task<List<SkillModel>> GetSkillsAsync();
    }
}
