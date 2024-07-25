using PricingCalculator.Data;
using PricingCalculator.Dtos.Requests.Products;
using PricingCalculator.Models;
using PricingCalculator.Models.Errors;
using PricingCalculator.Models.Results;
using PricingCalculator.Services.CategoryServices;

namespace PricingCalculator.Services.ProductServices
{
    public class ProductService(IUnitOfWork unitOfWork, ICategoryService categoryService) : IProductService
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly ICategoryService _categoryService = categoryService;
        public async Task<Result<Product, Error>> CreateProduct(CreateProductDTO createProductDTO)
        {
            try
            {
                var category = await _categoryService.GetCategoryById(createProductDTO.CategoryId);
                if (!category.IsSuccess)
                {
                    return Result<Product, Error>.Failure(category.Errors);
                }
                var product = createProductDTO.ToModel();
                var newProduct = await _unitOfWork.ProductRepository.CreateAsync(product);
                await _unitOfWork.SaveAsync();

                return newProduct;
            }
            catch
            {
                return Result<Product, Error>.Failure(Errors.ServerError);
            }
        }

        public async Task<Result<List<Product>, Error>> GetAllProducts()
        {
            try
            {
                var products = await _unitOfWork.ProductRepository.GetAllAsync();

                return products;
            }
            catch
            {
                return Result<List<Product>, Error>.Failure(Errors.ServerError);
            }
        }

        public async Task<Result<Product, Error>> GetProductById(Guid id)
        {
            try
            {
                var product = await _unitOfWork.ProductRepository.GetByIdAsync(id);

                if (product is null)
                    return Result<Product, Error>.Failure(Errors.ProductNotFound);

                return product;
            }
            catch
            {
                return Result<Product, Error>.Failure(Errors.ServerError);
            }
        }
    }
}
