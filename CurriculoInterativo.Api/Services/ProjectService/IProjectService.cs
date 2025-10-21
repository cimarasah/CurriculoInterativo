using CurriculoInterativo.Api.Models;

namespace CurriculoInterativo.Api.Services.ProjectService
{
    public interface IProjectService
    {
        Task<List<ProjectModel>> GetProjectsAsync();
        Task<List<ProjectModel>> GetProjectsWithCompanyAsync();
        Task<ListProjectBySkillModel> GetProjectsBySkillAsync(int idSkill);
    }
}
