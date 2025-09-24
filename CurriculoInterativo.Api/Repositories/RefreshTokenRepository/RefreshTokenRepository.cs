using CurriculoInterativo.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace CurriculoInterativo.Api.Repositories.RefreshTokenRepository
{
    public class RefreshTokenRepository : BaseRepository<RefreshToken>, IRefreshTokenRepository
    {
        public RefreshTokenRepository(ResumeDbContext context) : base(context)
        {
        }

        public async Task<RefreshToken?> GetByTokenAsync(string token)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(rt => rt.User)
                .FirstOrDefaultAsync(rt => rt.Token == token);
        }

        public async Task<List<RefreshToken>> GetByUserIdAsync(int userId)
        {
            return await _dbSet
                .AsNoTracking()
                .Where(rt => rt.UserId == userId && !rt.IsRevoked && rt.ExpiryDate > DateTime.UtcNow)
                .ToListAsync();
        }

        public async Task RevokeTokenAsync(string token)
        {
            var refreshToken = await _dbSet
                .FirstOrDefaultAsync(rt => rt.Token == token);

            if (refreshToken != null)
            {
                refreshToken.IsRevoked = true;
                _dbSet.Update(refreshToken);
                await _context.SaveChangesAsync();
            }
        }

        public async Task RevokeAllUserTokensAsync(int userId)
        {
            var userTokens = await _dbSet
                .Where(rt => rt.UserId == userId && !rt.IsRevoked)
                .ToListAsync();

            foreach (var token in userTokens)
            {
                token.IsRevoked = true;
            }

            if (userTokens.Any())
            {
                _dbSet.UpdateRange(userTokens);
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteExpiredTokensAsync()
        {
            var expiredTokens = await _dbSet
                .Where(rt => rt.ExpiryDate <= DateTime.UtcNow || rt.IsRevoked)
            .ToListAsync();

            if (expiredTokens.Any())
            {
                _dbSet.RemoveRange(expiredTokens);
                await _context.SaveChangesAsync();
            }
        }
    }
}
