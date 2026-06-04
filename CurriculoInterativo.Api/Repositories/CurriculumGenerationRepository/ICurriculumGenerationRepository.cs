using CurriculoInterativo.Api.Entities;

namespace CurriculoInterativo.Api.Repositories.CurriculumGenerationRepository
{
    public interface ICurriculumGenerationRepository : IBaseRepository<CurriculumGeneration>
    {
        /// <summary>
        /// Conta quantas gerações foram feitas hoje por um usuário autenticado
        /// </summary>
        Task<int> CountTodayByUserIdAsync(int userId);

        /// <summary>
        /// Conta quantas gerações foram feitas hoje por um IP
        /// </summary>
        Task<int> CountTodayByIpAddressAsync(string ipAddress);

        /// <summary>
        /// Verifica se o limite de 10 gerações por dia foi atingido
        /// </summary>
        Task<bool> HasReachedDailyLimitAsync(int? userId, string? ipAddress);

        /// <summary>
        /// Registra uma nova geração de currículo
        /// </summary>
        Task RegisterGenerationAsync(CurriculumGeneration generation);
    }
}
