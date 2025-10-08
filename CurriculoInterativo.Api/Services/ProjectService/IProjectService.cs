using CurriculoInterativo.Api.DTOs;

namespace CurriculoInterativo.Api.Services.ProjectService
{
    public interface IProjectService
    {
        Task<List<ProjectDto>> GetProjectsAsync();
        Task<List<ProjectDto>> GetProjectsWithCompanyAsync();
        Task<ListProjectBySkillDto> GetProjectsBySkillAsync(int idSkill);
    }
}
