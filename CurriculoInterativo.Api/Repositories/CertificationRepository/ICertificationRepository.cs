using CurriculoInterativo.Api.Enums;
using CurriculoInterativo.Api.Models;

namespace CurriculoInterativo.Api.Repositories.CertificationRepository
{
    public interface ICertificationRepository : IBaseRepository<Certification>
    {
        Task<IEnumerable<Certification>> GetByCategoryAsync(SkillCategory category);

    }
}
