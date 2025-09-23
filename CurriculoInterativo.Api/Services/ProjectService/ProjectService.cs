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
        public async Task<IEnumerable<ProjectDto>> GetProjectsBySkillAsync(SkillDto skillDto)
        {
            var skill =  _mapper.Map<Skill>(skillDto);
            var projects = await _repository.GetBySkillAsync(skill);

            return _mapper.Map<IEnumerable<ProjectDto>>(projects);
        }   
    }
}
