using CurriculoInterativo.Api.Entities;

namespace CurriculoInterativo.Api.Repositories.SuggestionRepository
{
    public interface ISuggestionRepository : IBaseRepository<Suggestion>
    {
        /// <summary>
        /// Busca um lead pelo email
        /// </summary>
        Task<Suggestion?> GetByEmailAsync(string email);

        /// <summary>
        /// Verifica se existe um lead com o email informado
        /// </summary>
        Task<bool> ExistsByEmailAsync(string email);
    }
}
