using AutoMapper;
using CurriculoInterativo.Api.DTOs;
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

        public async Task<List<ProjectDto>> GetProjectsAsync()
        {
            var projects = await _repository.GetAllAsync();             

            var projectDtos = _mapper.Map<List<ProjectDto>>(projects);

            return projectDtos;
        }
        public async Task<ListProjectBySkillDto> GetProjectsBySkillAsync(int idSkill)
        {
            var projects = await _repository.GetBySkillAsync(idSkill);

            if (projects == null || !projects.Any())
                return new ListProjectBySkillDto();

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

            return new ListProjectBySkillDto
            {
                SkillName = skillName,
                TotalMonths = totalMonths,
                Years = years,
                Months = months,
                Projects = _mapper.Map<List<ProjectDto>>(projects)
            };
        }
        public async Task<List<ProjectDto>> GetProjectsWithCompanyAsync()
        {
            var projects = await _repository.GetProjectsWithCompany();

            var projectDtos = _mapper.Map<List<ProjectDto>>(projects);

            return projectDtos;
        }
    }
}
