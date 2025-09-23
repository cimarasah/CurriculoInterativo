using AutoMapper;
using CurriculoInterativo.Api.DTOs;
using CurriculoInterativo.Api.Repositories.SkillRepository;

namespace CurriculoInterativo.Api.Services.SkillService
{
    public class SkillService : ISkillService
    {
        private readonly ISkillRepository _repository;
        private readonly IMapper _mapper;

        public SkillService(ISkillRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<List<SkillDto>> GetSkillsAsync()
        {
            var skills = await _repository.GetAllAsync();

            var skillDtos = _mapper.Map<List<SkillDto>>(skills);

            return skillDtos;
        }
        
    }
}
