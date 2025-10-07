using CurriculoInterativo.Api.Models;

namespace CurriculoInterativo.Api.Repositories.ProjectRepository
{
    public interface IProjectRepository : IBaseRepository<Project>
    {
        Task<IEnumerable<Project>> GetAllAsync();
        Task<IEnumerable<Project>> GetBySkillAsync(int idSkill);
        Task<IEnumerable<Project>> GetProjectsWithCompany();

    }
}
