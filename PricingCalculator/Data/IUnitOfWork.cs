using PricingCalculator.Data.Repositories.CategoryRepositories;
using PricingCalculator.Data.Repositories.ProductRepositories;

namespace PricingCalculator.Data
{
    public interface IUnitOfWork : IDisposable
    {
        public IProductRepository ProductRepository { get; }
        public ICategoryRepository CategoryRepository { get; }
        Task<int> SaveAsync();
        Task BeginTransactionAsync();
        Task CommitAsync();
        Task RollbackAsync();
    }
}
