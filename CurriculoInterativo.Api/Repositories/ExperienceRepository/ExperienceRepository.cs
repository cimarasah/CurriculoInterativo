using CurriculoInterativo.Api.DTOs;
using CurriculoInterativo.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace CurriculoInterativo.Api.Repositories.ExperienceRepository
{
    public class ExperienceRepository
    : BaseRepository<Experience>, IExperienceRepository
    {
        public ExperienceRepository(ResumeDbContext context)
            : base(context)
        {
        }

        public async Task<List<Experience>> GetExperiencesWithProjectsAsync()
        {
            return await _dbSet
                .Include(e => e.Projects)
                    .ThenInclude(p => p.Skills)
                .ToListAsync();
        }
    }
}
