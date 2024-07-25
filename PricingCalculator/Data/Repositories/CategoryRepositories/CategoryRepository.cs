using PricingCalculator.Models;

namespace PricingCalculator.Data.Repositories.CategoryRepositories
{
    public class CategoryRepository(DataContext context) 
        : GenericRepository<int, Category>(context) ,ICategoryRepository
    {
    }
}
