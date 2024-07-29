using Microsoft.EntityFrameworkCore;
using PricingCalculator.Models;

namespace PricingCalculator.Data.Repositories.CategoryRepositories
{
    public class CategoryRepository(DataContext context)
        : GenericRepository<int, Category>(context), ICategoryRepository
    {
        private readonly DataContext _context = context;

        public async Task<List<Category>> GetAllCategoriesWithProducts()
        {
            return await _context.Categories
                                .Include(c => c.Products)
                                .ToListAsync();
        }
    }
}
