using AutoMapper;
using CurriculoInterativo.Api.DTOs;
using CurriculoInterativo.Api.Enums;
using CurriculoInterativo.Api.Repositories.CertificationRepository;

namespace CurriculoInterativo.Api.Services.CertificationService
{
    public class CertificationService : ICertificationService
    {
        private readonly ICertificationRepository _repository;
        private readonly IMapper _mapper;

        public CertificationService(ICertificationRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<List<CertificationDto>> GetCertificationsAsync()
        {
            var certifications = await _repository.GetAllAsync();

            var certificationDtos = _mapper.Map<List<CertificationDto>>(certifications);

            return certificationDtos;
        }
        public async Task<IEnumerable<CertificationDto>> GetCertificationsByCategoryAsync(SkillCategory category)
        {
            var certifications = await _repository.GetByCategoryAsync(category);

            return _mapper.Map<IEnumerable<CertificationDto>>(certifications);
        }   
    }
}
