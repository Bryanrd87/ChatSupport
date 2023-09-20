using ChatSupport.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ChatSupport.Persistence.Repositories;
public class Repository<T> : IRepository<T> where T : class
{
    private readonly ApplicationDbContext _context;
    private readonly DbSet<T> _dbSet;

    public Repository(ApplicationDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public async Task AddAsync(T entity, CancellationToken cancellationToken)
    {
        await _dbSet.AddAsync(entity, cancellationToken);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken)
    {
        var entity = await _dbSet.FindAsync(new object?[] { id, cancellationToken }, cancellationToken: cancellationToken);
        if (entity is not null)
            _dbSet.Remove(entity);        
    }

    public async Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken, string includeProperties)
    {
        IQueryable<T> query = _dbSet; 
        if (includeProperties is not null)
        {
            foreach (var includeProperty in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }
        }
        return await query.ToListAsync(cancellationToken);
    }

    public async Task<T> GetByIdAsync(
    object key,
    CancellationToken cancellationToken, string includeProperties)
    {
        var entity = await _context.Set<T>().FindAsync(key, cancellationToken);

        if (entity is not null && includeProperties is not null)
        {
            foreach (var include in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                _context.Entry(entity).Reference(include).Load();
            }
        }

        return entity;
    }  

    public async Task UpdateAsync(T entity, CancellationToken cancellationToken)
    {
        _dbSet.Attach(entity);
        _context.Entry(entity).State = EntityState.Modified;
    }
}

