using CurriculoInterativo.Api.Models;

namespace CurriculoInterativo.Api.Repositories.ProjectRepository
{
    public interface IProjectRepository : IBaseRepository<Project>
    {
        Task<IEnumerable<Project>> GetBySkillAsync(Skill skill);

    }
}
