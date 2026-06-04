using CurriculoInterativo.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace CurriculoInterativo.Api.Repositories.CurriculumGenerationRepository
{
    public class CurriculumGenerationRepository : BaseRepository<CurriculumGeneration>, ICurriculumGenerationRepository
    {
        private const int DAILY_LIMIT = 10;

        public CurriculumGenerationRepository(ResumeDbContext context)
            : base(context)
        {
        }

        public async Task<int> CountTodayByUserIdAsync(int userId)
        {
            var today = DateTime.UtcNow.Date;
            return await _dbSet
                .Where(g => g.UserId == userId && g.GeneratedAt.Date == today)
                .CountAsync();
        }

        public async Task<int> CountTodayByIpAddressAsync(string ipAddress)
        {
            var today = DateTime.UtcNow.Date;
            return await _dbSet
                .Where(g => g.IpAddress == ipAddress && g.GeneratedAt.Date == today)
                .CountAsync();
        }

        public async Task<bool> HasReachedDailyLimitAsync(int? userId, string? ipAddress)
        {
            var today = DateTime.UtcNow.Date;

            if (userId.HasValue)
            {
                var count = await _dbSet
                    .Where(g => g.UserId == userId && g.GeneratedAt.Date == today)
                    .CountAsync();
                return count >= DAILY_LIMIT;
            }

            if (!string.IsNullOrEmpty(ipAddress))
            {
                var count = await _dbSet
                    .Where(g => g.IpAddress == ipAddress && g.GeneratedAt.Date == today)
                    .CountAsync();
                return count >= DAILY_LIMIT;
            }

            return false;
        }

        public async Task RegisterGenerationAsync(CurriculumGeneration generation)
        {
            await AddAsync(generation);
        }
    }
}
