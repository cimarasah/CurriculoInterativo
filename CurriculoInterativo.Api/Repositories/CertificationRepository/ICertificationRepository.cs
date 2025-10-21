using CurriculoInterativo.Api.Enums;
using CurriculoInterativo.Api.Entities;

namespace CurriculoInterativo.Api.Repositories.CertificationRepository
{
    public interface ICertificationRepository : IBaseRepository<Certification>
    {
        Task<IEnumerable<Certification>> GetByCategoryAsync(SkillCategory category);

    }
}
