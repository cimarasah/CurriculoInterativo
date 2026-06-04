using CurriculoInterativo.Api.Entities;

namespace CurriculoInterativo.Api.Repositories.PasswordResetTokenRepository
{
    public interface IPasswordResetTokenRepository : IBaseRepository<PasswordResetToken>
    {
        Task<PasswordResetToken?> GetByTokenAsync(string token);
        Task<PasswordResetToken?> GetValidTokenByUserIdAsync(int userId);
        Task InvalidateUserTokensAsync(int userId);
    }
}

