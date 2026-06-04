using CurriculoInterativo.Api.Entities;
using CurriculoInterativo.Api.Repositories;
using Microsoft.EntityFrameworkCore;

namespace CurriculoInterativo.Api.Repositories.PasswordResetTokenRepository
{
    public class PasswordResetTokenRepository : BaseRepository<PasswordResetToken>, IPasswordResetTokenRepository
    {
        public PasswordResetTokenRepository(ResumeDbContext context) : base(context)
        {
        }

        public async Task<PasswordResetToken?> GetByTokenAsync(string token)
        {
            return await _dbSet
                .Include(t => t.User)
                .FirstOrDefaultAsync(t => t.Token == token);
        }

        public async Task<PasswordResetToken?> GetValidTokenByUserIdAsync(int userId)
        {
            return await _dbSet
                .Where(t => t.UserId == userId 
                    && !t.IsUsed 
                    && t.ExpiresAt > DateTime.UtcNow)
                .OrderByDescending(t => t.CreatedAt)
                .FirstOrDefaultAsync();
        }

        public async Task InvalidateUserTokensAsync(int userId)
        {
            var tokens = await _dbSet
                .Where(t => t.UserId == userId && !t.IsUsed)
                .ToListAsync();

            foreach (var token in tokens)
            {
                token.IsUsed = true;
                token.UsedAt = DateTime.UtcNow;
            }

            if (tokens.Any())
            {
                await _context.SaveChangesAsync();
            }
        }
    }
}

