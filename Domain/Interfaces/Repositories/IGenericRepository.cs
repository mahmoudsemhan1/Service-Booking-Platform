using System.Linq.Expressions;

namespace Domain.Interfaces.Repositories
{
    public interface IGenericRepository<T> where T : class
    {
        Task<T?> GetByIdAsync(int id);
        Task<IEnumerable<T>> GetAllAsync();
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
        Task<T?> FindOneAsync(Expression<Func<T, bool>> predicate);
        Task<bool> AnyAsync(Expression<Func<T, bool>> predicate);
        Task AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(T entity);

        Task<bool> ExistsAsync(int id);

        // the following method is
        // this spcific for pagination , add a method to get paged results
        // Paged retrieval method  , includeProperties, predicate, pageNumber, pageSize 
        Task<(IEnumerable<T> Items, int TotalCount)> GetPagedAsync(
         int pageNumber,
         int pageSize,
         Expression<Func<T, bool>>? predicate = null,
         string? includeProperties = null);



    }
}
