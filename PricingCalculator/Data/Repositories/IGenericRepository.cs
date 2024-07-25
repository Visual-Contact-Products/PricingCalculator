namespace PricingCalculator.Data.Repositories
{
    public interface IGenericRepository<TId, TEntity> where TEntity : class, new()
    {
        Task<TEntity> CreateAsync(TEntity entity);

        Task<List<TEntity>> GetAllAsync();
        IQueryable<TEntity> GetQueryable();

        Task<TEntity?> GetByIdAsync(TId id);

        void Update(TEntity enitity);
    }
}
