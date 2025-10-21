using AutoMapper;
using CurriculoInterativo.Api.Models;
using CurriculoInterativo.Api.Repositories.ProjectRepository;

namespace CurriculoInterativo.Api.Services.ProjectService
{
    public class ProjectService : IProjectService
    {
        private readonly IProjectRepository _repository;
        private readonly IMapper _mapper;

        public ProjectService(IProjectRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<List<ProjectModel>> GetProjectsAsync()
        {
            var projects = await _repository.GetAllAsync();             

            var projectDtos = _mapper.Map<List<ProjectModel>>(projects);

            return projectDtos;
        }
        public async Task<ListProjectBySkillModel> GetProjectsBySkillAsync(int idSkill)
        {
            var projects = await _repository.GetBySkillAsync(idSkill);

            if (projects == null || !projects.Any())
                return new ListProjectBySkillModel();

            var skillName = projects.First().Skills.FirstOrDefault(s => s.Id == idSkill)?.Name ?? string.Empty;

            int totalMonths = 0;

            foreach (var project in projects)
            {
                var start = project.StartDate;
                var end = project.EndDate ?? DateTime.UtcNow;

                if (end < start)
                    continue;

                totalMonths += ((end.Year - start.Year) * 12) + (end.Month - start.Month);
            }

            var years = totalMonths / 12;
            var months = totalMonths % 12;

            return new ListProjectBySkillModel
            {
                SkillName = skillName,
                TotalMonths = totalMonths,
                Years = years,
                Months = months,
                Projects = _mapper.Map<List<ProjectModel>>(projects)
            };
        }
        public async Task<List<ProjectModel>> GetProjectsWithCompanyAsync()
        {
            var projects = await _repository.GetProjectsWithCompany();

            var projectDtos = _mapper.Map<List<ProjectModel>>(projects);

            return projectDtos;
        }
    }
}
