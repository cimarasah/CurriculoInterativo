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

        public async Task<List<ExperienceLogoDto>> GetExperiencesLogoAsync()
        {
            var experiences = await _repository.GetAllAsync();

            var experienceDtos = _mapper.Map<List<ExperienceLogoDto>>(experiences);

            return experienceDtos;
        }
    }
}
