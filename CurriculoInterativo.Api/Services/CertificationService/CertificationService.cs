using AutoMapper;
using CurriculoInterativo.Api.DTOs;
using CurriculoInterativo.Api.Enums;
using CurriculoInterativo.Api.Models;
using CurriculoInterativo.Api.Repositories.CertificationRepository;
using CurriculoInterativo.Api.Utils.Exceptions;
using System.ComponentModel.DataAnnotations;

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
        public async Task<CertificationDto?> GetCertificationByIdAsync(int id)
        {
            var certification = await _repository.GetByIdAsync(id);
            if (certification == null)
                throw new NotFoundException($"Certificação com ID {id} não encontrado");

            return _mapper.Map<CertificationDto>(certification);
        }

        public async Task<CertificationDto> CreateCertificationAsync(CertificationDto certificationDto, int userId)
        {
            if (certificationDto == null)
                throw new ValidationException("Dados do Certificação não podem ser nulos");

            var certification = _mapper.Map<Certification>(certificationDto);

            await _repository.AddAsync(certification);

            return _mapper.Map<CertificationDto>(certification);
        }

        public async Task<CertificationDto?> UpdateCertificationAsync(int id, CertificationDto certificationDto, int userId)
        {
            var certification = await _repository.GetByIdAsync(id);
            if (certification == null)
                throw new NotFoundException($"Certificação com ID {id} não encontrado");

            _mapper.Map(certificationDto, certification);

            await _repository.UpdateAsync(certification);

            return _mapper.Map<CertificationDto>(certification);
        }

        public async Task<bool> DeleteCertificationAsync(int id, int userId)
        {
            var certification = await _repository.GetByIdAsync(id);
            if (certification == null)
                throw new NotFoundException($"Certificação com ID {id} não encontrado");

            await _repository.DeleteAsync(id);
            return true;
        }
    }
}
