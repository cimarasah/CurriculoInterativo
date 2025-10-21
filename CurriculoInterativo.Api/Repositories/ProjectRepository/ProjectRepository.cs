using CurriculoInterativo.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace CurriculoInterativo.Api.Repositories.ProjectRepository
{
    public class ProjectRepository
   : BaseRepository<Project>, IProjectRepository
    {
        public ProjectRepository(ResumeDbContext context)
            : base(context)
        {
        }
        public override async Task<IEnumerable<Project>> GetAllAsync()
        {
            return await _context.Projects
                .Include(p => p.Skills)
                .Include(p => p.Responsibilities)
                .ToListAsync();
        }
        public async Task<IEnumerable<Project>> GetBySkillAsync(int idSkill)
        {
            return await _context.Projects
            .Include(p => p.Skills)
            .Where(p => p.Skills.Any(s => s.Id == idSkill))
            .ToListAsync();
        }

        public async Task<IEnumerable<Project>> GetProjectsWithCompany()
        {
            return await _context.Projects
        .Include(p => p.Experience) // para trazer a empresa
        .Include(p => p.Skills) // caso queira trazer skills
        .Select(p => new Project
        {
            Id = p.Id,
            Name = p.Name,
            Description = p.Description,
            Position = p.Position,
            Experience = new Experience
            {
                Company = p.Experience.Company 
            },
            StartDate = p.StartDate,
            EndDate = p.EndDate,
            Skills = p.Skills
                .Select(s => new Skill
                {
                    Id = s.Id,
                    Name = s.Name,
                    Category = s.Category,
                    IsCurrentlyUsed = s.IsCurrentlyUsed,
                    StartDate = s.StartDate,
                }).ToList()
        })
        .ToListAsync();
        }
    }
}