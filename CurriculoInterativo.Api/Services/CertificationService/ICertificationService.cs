using CurriculoInterativo.Api.DTOs;
using CurriculoInterativo.Api.Enums;

namespace CurriculoInterativo.Api.Services.CertificationService
{
    public interface ICertificationService
    {
        Task<List<CertificationDto>> GetCertificationsAsync();
        Task<IEnumerable<CertificationDto>> GetCertificationsByCategoryAsync(SkillCategory category);
        Task<CertificationDto?> GetCertificationByIdAsync(int id);

        Task<CertificationDto> CreateCertificationAsync(CertificationDto certificationDto, int userId);

        Task<CertificationDto?> UpdateCertificationAsync(int id, CertificationDto certificationDto, int userId);

        Task<bool> DeleteCertificationAsync(int id, int userId);
    }
}
