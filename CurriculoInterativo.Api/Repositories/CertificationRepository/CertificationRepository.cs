using CurriculoInterativo.Api.Enums;
using CurriculoInterativo.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace CurriculoInterativo.Api.Repositories.CertificationRepository
{
    public class CertificationRepository(ResumeDbContext context) : BaseRepository<Certification>(context), ICertificationRepository
    {
        public async Task<IEnumerable<Certification>> GetByCategoryAsync(SkillCategory category)
        {
            return await _context.Certifications
                .Where(c => c.Category == category)
                .ToListAsync();
        }
    }
}
