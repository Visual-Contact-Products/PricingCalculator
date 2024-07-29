using NSubstitute;
using NSubstitute.ExceptionExtensions;
using NSubstitute.ReturnsExtensions;
using PricingCalculator.Data;
using PricingCalculator.Dtos.Requests.Products;
using PricingCalculator.Models;
using PricingCalculator.Models.Errors;
using PricingCalculator.Models.Results;
using PricingCalculator.Services.CategoryServices;
using PricingCalculator.Services.ProductServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PricingCalculator.Tests.Products
{
    public class ProductServiceTests
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICategoryService _categoryService;

        public ProductServiceTests()
        {
            _unitOfWork = Substitute.For<IUnitOfWork>();
            _categoryService = Substitute.For<ICategoryService>();
        }

        public ProductService CreateProductService()
        {
            return new ProductService(_unitOfWork, _categoryService);
        }

        [Fact]
        public async Task CreateProduct_CategoryDoesNotExists_returnsError()
        {
            // Arrange
            var createProductDTO = new CreateProductDTO()
            {
                CategoryId = 1
            };

            _categoryService.GetCategoryById(Arg.Any<int>())
                .Returns(Result<Category, Error>.Failure(Errors.CategoryNotFound));

            var productService = CreateProductService();
            // Act
            var result = await productService.CreateProduct(createProductDTO);

            // Assert
            Assert.Equal(result.Errors[0], Errors.CategoryNotFound);
        }

        [Fact]
        public async Task CreateProduct_Success_returnsNewProduct()
        {
            // Arrange
            var createProductDTO = new CreateProductDTO()
            {
                CategoryId = 1
            };

            var category = new Category()
            {
                Id = createProductDTO.CategoryId
            };

            var product = new Product();

            _categoryService.GetCategoryById(Arg.Any<int>())
                .Returns(Result<Category, Error>.Success(category));

            _unitOfWork.ProductRepository.CreateAsync(Arg.Any<Product>()).Returns(product);

            var productService = CreateProductService();
            // Act
            var result = await productService.CreateProduct(createProductDTO);

            // Assert
            Assert.True(result.IsSuccess);
        }

        [Fact]
        public async Task GetAllProducts_Success_ReturnsListOfProducts()
        {
            // Arrange
            var products = new List<Product>
    {
        new Product { Id = Guid.NewGuid(), Description = "Product1" },
        new Product { Id = Guid.NewGuid(), Description = "Product2" }
    };

            _unitOfWork.ProductRepository.GetAllAsync().Returns(products);

            var productService = CreateProductService();

            // Act
            var result = await productService.GetAllProducts();

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(products, result.Value);
        }

        [Fact]
        public async Task GetAllProducts_Failure_ReturnsServerError()
        {
            // Arrange
            _unitOfWork.ProductRepository.GetAllAsync().Throws(new Exception());

            var productService = CreateProductService();

            // Act
            var result = await productService.GetAllProducts();

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(Errors.ServerError, result.Errors[0]);
        }

        [Fact]
        public async Task GetProductById_Success_ReturnsProduct()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var product = new Product { Id = productId, Description = "Product1" };

            _unitOfWork.ProductRepository.GetByIdAsync(productId).Returns(product);

            var productService = CreateProductService();

            // Act
            var result = await productService.GetProductById(productId);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(product, result.Value);
        }

        [Fact]
        public async Task GetProductById_NotFound_ReturnsProductNotFound()
        {
            // Arrange
            var productId = Guid.NewGuid();

            _unitOfWork.ProductRepository.GetByIdAsync(productId).ReturnsNull();

            var productService = CreateProductService();

            // Act
            var result = await productService.GetProductById(productId);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(Errors.ProductNotFound, result.Errors[0]);
        }

        [Fact]
        public async Task GetProductById_Failure_ReturnsServerError()
        {
            // Arrange
            var productId = Guid.NewGuid();

            _unitOfWork.ProductRepository.GetByIdAsync(productId).Throws(new Exception());

            var productService = CreateProductService();

            // Act
            var result = await productService.GetProductById(productId);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(Errors.ServerError, result.Errors[0]);
        }
    }
}
