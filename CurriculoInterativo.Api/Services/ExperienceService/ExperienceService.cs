using AutoMapper;
using CurriculoInterativo.Api.DTOs.ExperienceDto;
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

        public async Task<List<ExperienceResponse>> GetExperiencesMiniAsync()
        {
            var experiences = await _repository.GetAllAsync();

            var experienceDtos = _mapper.Map<List<ExperienceResponse>>(experiences);

            return experienceDtos;
        }
    }
}
