using NSubstitute;
using PricingCalculator.Data;
using PricingCalculator.Dtos.Requests.Categories;
using PricingCalculator.Models.Errors;
using PricingCalculator.Models;
using PricingCalculator.Services.CategoryServices;
using PricingCalculator.Services.ProductServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NSubstitute.ExceptionExtensions;
using NSubstitute.ReturnsExtensions;

namespace PricingCalculator.Tests.Categories
{
    public class CategoryServiceTests
    {
        private readonly IUnitOfWork _unitOfWork;

        public CategoryServiceTests()
        {
            _unitOfWork = Substitute.For<IUnitOfWork>();
        }

        public CategoryService CreateCategoryService()
        {
            return new CategoryService(_unitOfWork);
        }

        [Fact]
        public async Task CreateCategory_Success_ReturnsNewCategory()
        {
            // Arrange
            var createCategoryDTO = new CreateCategoryDTO { Name = "New Category" };
            var category = createCategoryDTO.ToModel();

            var newCategory = new Category { Id = 1, Name = "New Category" };

            _unitOfWork.CategoryRepository.CreateAsync(Arg.Any<Category>()).Returns(newCategory);

            var categoryService = CreateCategoryService();

            // Act
            var result = await categoryService.CreateCategory(createCategoryDTO);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(newCategory, result.Value);
        }

        [Fact]
        public async Task CreateCategory_Failure_ReturnsServerError()
        {
            // Arrange
            var createCategoryDTO = new CreateCategoryDTO { Name = "New Category" };

            _unitOfWork.CategoryRepository.CreateAsync(Arg.Any<Category>()).Throws(new Exception());

            var categoryService = CreateCategoryService();

            // Act
            var result = await categoryService.CreateCategory(createCategoryDTO);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(Errors.ServerError, result.Errors[0]);
        }

        [Fact]
        public async Task GetAllCategoriesWithProducts_Success_ReturnsCategories()
        {
            // Arrange
            var categories = new List<Category>
            {
                new Category { Id = 1, Name = "Category1" },
                new Category { Id = 2, Name = "Category2" }
            };

            _unitOfWork.CategoryRepository.GetAllCategoriesWithProducts().Returns(categories);

            var categoryService = CreateCategoryService();

            // Act
            var result = await categoryService.GetAllCategoriesWithProducts();

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(categories, result.Value);
        }

        [Fact]
        public async Task GetAllCategoriesWithProducts_Failure_ReturnsServerError()
        {
            // Arrange
            _unitOfWork.CategoryRepository.GetAllCategoriesWithProducts().Throws(new Exception());

            var categoryService = CreateCategoryService();

            // Act
            var result = await categoryService.GetAllCategoriesWithProducts();

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(Errors.ServerError, result.Errors[0]);
        }

        [Fact]
        public async Task GetCategoryById_Success_ReturnsCategory()
        {
            // Arrange
            var categoryId = 1;
            var category = new Category { Id = categoryId, Name = "Category1" };

            _unitOfWork.CategoryRepository.GetByIdAsync(categoryId).Returns(category);

            var categoryService = CreateCategoryService();

            // Act
            var result = await categoryService.GetCategoryById(categoryId);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(category, result.Value);
        }

        [Fact]
        public async Task GetCategoryById_NotFound_ReturnsCategoryNotFound()
        {
            // Arrange
            var categoryId = 1;

            _unitOfWork.CategoryRepository.GetByIdAsync(categoryId).ReturnsNull();

            var categoryService = CreateCategoryService();

            // Act
            var result = await categoryService.GetCategoryById(categoryId);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(Errors.CategoryNotFound, result.Errors[0]);
        }

        [Fact]
        public async Task GetCategoryById_Failure_ReturnsServerError()
        {
            // Arrange
            var categoryId = 1;

            _unitOfWork.CategoryRepository.GetByIdAsync(categoryId).Throws(new Exception());

            var categoryService = CreateCategoryService();

            // Act
            var result = await categoryService.GetCategoryById(categoryId);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(Errors.ServerError, result.Errors[0]);
        }

    }
}
