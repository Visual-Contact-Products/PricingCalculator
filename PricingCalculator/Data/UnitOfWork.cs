using Microsoft.EntityFrameworkCore.Storage;
using PricingCalculator.Data.Repositories.CategoryRepositories;
using PricingCalculator.Data.Repositories.ProductRepositories;

namespace PricingCalculator.Data
{
    public class UnitOfWork(DataContext context, 
        ICategoryRepository categoryRepository, 
        IProductRepository productRepository) : IUnitOfWork
    {
        private readonly DataContext _context = context;
        private IDbContextTransaction _currentTransaction = null;

        public IProductRepository ProductRepository { get; } = productRepository;
        public ICategoryRepository CategoryRepository { get; } = categoryRepository;

        public void Dispose()
        {
            _context.Dispose();
        }

        public async Task BeginTransactionAsync()
        {
            _currentTransaction = await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitAsync()
        {
            try
            {
                await _context.SaveChangesAsync();
                await _currentTransaction.CommitAsync();
            }
            catch
            {
                await _currentTransaction.RollbackAsync();
            }
            finally
            {
                await _currentTransaction.DisposeAsync();
                _currentTransaction = null;
            }
        }

        public async Task RollbackAsync()
        {
            try
            {
                await _currentTransaction.RollbackAsync();
            }
            finally
            {
                await _currentTransaction.DisposeAsync();
                _currentTransaction = null;
            }
        }

        public async Task<int> SaveAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
