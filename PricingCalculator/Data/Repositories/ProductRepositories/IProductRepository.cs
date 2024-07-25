using PricingCalculator.Models;

namespace PricingCalculator.Data.Repositories.ProductRepositories
{
    public interface IProductRepository : IGenericRepository<Guid, Product>
    {
    }
}
