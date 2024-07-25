using PricingCalculator.Dtos.Requests.Categories;
using PricingCalculator.Models;
using PricingCalculator.Models.Errors;
using PricingCalculator.Models.Results;

namespace PricingCalculator.Services.CategoryServices
{
    public interface ICategoryService
    {
        public Task<Result<List<Category>, Error>> GetAllCategoriesWithProducts();
        public Task<Result<Category, Error>> GetCategoryById(int id);
        public Task<Result<Category, Error>> CreateCategory(CreateCategoryDTO createCategoryDTO);
    }
}
