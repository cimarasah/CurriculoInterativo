using CurriculoInterativo.Api.Entities;

namespace CurriculoInterativo.Api.Repositories.ExperienceRepository
{
    public interface IExperienceRepository : IBaseRepository<Experience>
    {
        Task<List<Experience>> GetExperiencesWithProjectsAsync();
    }
}
