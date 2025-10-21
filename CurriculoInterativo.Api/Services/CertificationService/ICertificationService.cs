using CurriculoInterativo.Api.Models;
using CurriculoInterativo.Api.Enums;

namespace CurriculoInterativo.Api.Services.CertificationService
{
    public interface ICertificationService
    {
        Task<List<CertificationModel>> GetCertificationsAsync();
        Task<IEnumerable<CertificationModel>> GetCertificationsByCategoryAsync(SkillCategory category);
        Task<CertificationModel?> GetCertificationByIdAsync(int id);

        Task<CertificationModel> CreateCertificationAsync(CertificationModel certificationDto, int userId);

        Task<CertificationModel?> UpdateCertificationAsync(int id, CertificationModel certificationDto, int userId);

        Task<bool> DeleteCertificationAsync(int id, int userId);
    }
}
