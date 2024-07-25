using Microsoft.EntityFrameworkCore;
using PricingCalculator.Data;
using PricingCalculator.Dtos.Requests.Categories;
using PricingCalculator.Models;
using PricingCalculator.Models.Errors;
using PricingCalculator.Models.Results;

namespace PricingCalculator.Services.CategoryServices
{
    public class CategoryService(DataContext context, IUnitOfWork unitOfWork) : ICategoryService
    {
        private readonly DataContext _context = context;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<Result<Category, Error>> CreateCategory(CreateCategoryDTO createCategoryDTO)
        {
            try
            {
                var category = createCategoryDTO.ToModel();

                var newCategory = await _unitOfWork.CategoryRepository.CreateAsync(category);
                await _unitOfWork.SaveAsync();

                return newCategory;
            }
            catch
            {
                return Result<Category, Error>.Failure(Errors.ServerError);
            }
        }

        public async Task<Result<List<Category>, Error>> GetAllCategoriesWithProducts()
        {
            try
            {
                var categories = await _context.Categories
                                .Include(c => c.Products)
                                .ToListAsync();

                return categories;
            }
            catch
            {
                return Result<List<Category>, Error>.Failure(Errors.ServerError);
            }
            
        }

        public async Task<Result<Category, Error>> GetCategoryById(int id)
        {
            var category = await _unitOfWork.CategoryRepository.GetByIdAsync(id);

            if (category is null)
                return Result<Category, Error>.Failure(Errors.CategoryNotFound);

            return category;
        }
    }
}
