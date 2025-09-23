using CurriculoInterativo.Api.Models;

namespace CurriculoInterativo.Api.Repositories.ExperienceRepository
{
    public class ExperienceRepository(ResumeDbContext context) : BaseRepository<Experience>(context), IExperienceRepository
    {
    }
}
