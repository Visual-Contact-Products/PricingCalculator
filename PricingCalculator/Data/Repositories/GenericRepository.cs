using Microsoft.EntityFrameworkCore;

namespace PricingCalculator.Data.Repositories
{
    public abstract class GenericRepository<TId, TEntity>(DataContext context) 
        : IGenericRepository<TId, TEntity> where TEntity: class, new()
    {
        private readonly DataContext _context = context;
        public virtual async Task<TEntity> CreateAsync(TEntity entity)
        {
            await _context.Set<TEntity>().AddAsync(entity);
            return entity;
        }

        public virtual async Task<List<TEntity>> GetAllAsync()
        {
            return await _context.Set<TEntity>().ToListAsync();
        }

        public virtual async Task<TEntity?> GetByIdAsync(TId id)
        {
            return await _context.Set<TEntity>().FindAsync(id);
        }

        public virtual IQueryable<TEntity> GetQueryable()
        {
            return _context.Set<TEntity>();
        }

        public virtual void Update(TEntity entity)
        {
            _context.Set<TEntity>().Update(entity);
        }
    }
}
