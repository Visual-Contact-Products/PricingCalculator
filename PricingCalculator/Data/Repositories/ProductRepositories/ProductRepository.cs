using PricingCalculator.Models;

namespace PricingCalculator.Data.Repositories.ProductRepositories
{
    public class ProductRepository : GenericRepository<Guid, Product>, IProductRepository
    {
        private readonly DataContext _context;

        public ProductRepository(DataContext context) : base(context)
        {
            _context = context;
        }
    }
}
