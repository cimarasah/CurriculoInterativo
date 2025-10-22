using CurriculoInterativo.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace CurriculoInterativo.Api.Repositories.SuggestionRepository
{
    public class SuggestionRepository : BaseRepository<Suggestion>, ISuggestionRepository
    {
        public SuggestionRepository(ResumeDbContext context) : base(context)
        {
        }

        public async Task<Suggestion?> GetByEmailAsync(string email)
        {
            return await _dbSet
                .AsNoTracking()
                .FirstOrDefaultAsync(l => l.Email.ToLower() == email.ToLower());
        }

        public async Task<bool> ExistsByEmailAsync(string email)
        {
            return await _dbSet
                .AsNoTracking()
                .AnyAsync(l => l.Email.ToLower() == email.ToLower());
        }

    }
}
