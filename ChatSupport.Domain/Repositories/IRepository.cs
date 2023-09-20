namespace ChatSupport.Domain.Repositories;
public interface IRepository<T> where T : class
{
    Task<T> GetByIdAsync(object key, CancellationToken cancellationToken, string include = null);
    Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken, string include = null);
    Task AddAsync(T entity,CancellationToken cancellationToken);
    Task UpdateAsync(T entity, CancellationToken cancellationToken);
    Task DeleteAsync(int id, CancellationToken cancellationToken);
}

