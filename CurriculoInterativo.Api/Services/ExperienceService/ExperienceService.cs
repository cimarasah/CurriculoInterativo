using AutoMapper;
using CurriculoInterativo.Api.DTOs;
using CurriculoInterativo.Api.Repositories.ExperienceRepository;

namespace CurriculoInterativo.Api.Services.ExperienceService
{
    public class ExperienceService : IExperienceService
    {
        private readonly IExperienceRepository _repository;
        private readonly IMapper _mapper;

        public ExperienceService(IExperienceRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<List<ExperienceDto>> GetExperiencesAsync()
        {
            var experiences = await _repository.GetExperiencesWithProjectsAsync();

            var experienceDtos = _mapper.Map<List<ExperienceDto>>(experiences);

            return experienceDtos;
        }
    }
}
