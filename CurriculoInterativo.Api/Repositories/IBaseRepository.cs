using Microsoft.EntityFrameworkCore;

namespace CurriculoInterativo.Api.Repositories
{
    public interface IBaseRepository<TEntity> where TEntity : class
    {
        DbSet<TEntity> GetDbSet();
        Task<IEnumerable<TEntity>> GetAllAsync();
        Task<TEntity?> GetByIdAsync(int id);
        Task AddAsync(TEntity entity);
        Task UpdateAsync(TEntity entity);
        Task DeleteAsync(int id);
    }
}
