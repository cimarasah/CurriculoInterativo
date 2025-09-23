using CurriculoInterativo.Api.DTOs;
using CurriculoInterativo.Api.Enums;

namespace CurriculoInterativo.Api.Services.CertificationService
{
    public interface ICertificationService
    {
        Task<List<CertificationDto>> GetCertificationsAsync();
        Task<IEnumerable<CertificationDto>> GetCertificationsByCategoryAsync(SkillCategory category);
    }
}
