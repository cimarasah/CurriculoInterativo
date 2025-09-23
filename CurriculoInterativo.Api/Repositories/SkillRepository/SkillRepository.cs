using CurriculoInterativo.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace CurriculoInterativo.Api.Repositories.SkillRepository
{
    public class SkillRepository(ResumeDbContext context) : BaseRepository<Skill>(context), ISkillRepository
    {
    }
}
