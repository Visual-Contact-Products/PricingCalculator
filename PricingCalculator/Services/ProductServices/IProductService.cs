using PricingCalculator.Dtos.Requests.Products;
using PricingCalculator.Models;
using PricingCalculator.Models.Errors;
using PricingCalculator.Models.Results;

namespace PricingCalculator.Services.ProductServices
{
    public interface IProductService
    {
        public Task<Result<Product, Error>> CreateProduct(CreateProductDTO createProductDTO);
        public Task<Result<List<Product>, Error>> GetAllProducts();
        public Task<Result<Product, Error>> GetProductById(Guid id);
    }
}
