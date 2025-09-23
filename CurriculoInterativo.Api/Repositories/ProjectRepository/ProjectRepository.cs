using CurriculoInterativo.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace CurriculoInterativo.Api.Repositories.ProjectRepository
{
    public class ProjectRepository(ResumeDbContext context) : BaseRepository<Project>(context), IProjectRepository
    {
        public override async Task<IEnumerable<Project>> GetAllAsync()
        {
            return await _context.Projects
                .Include(p => p.Skills)
                .Include(p => p.Responsibilities)
                .ToListAsync();
        }
        public async Task<IEnumerable<Project>> GetBySkillAsync(Skill skill)
        {
            return await _context.Projects
                .Include(p => p.Skills)
                .Where(p => p.Skills.Contains(skill))
                .ToListAsync();
        }
    }
}
