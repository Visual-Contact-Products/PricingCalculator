using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PricingCalculator.Dtos.Requests.Categories;
using PricingCalculator.Services.CategoryServices;

namespace PricingCalculator.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet("with-products")]
        public async Task<IActionResult> GetCategoriesWithProducts()
        {
            var result = await _categoryService.GetAllCategoriesWithProducts();

            if (!result.IsSuccess)
            {
                return StatusCode(result.Errors[0].HttpStatusCode, result);
            }

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateCategory(CreateCategoryDTO createCategoryDTO)
        {
            var result = await _categoryService.CreateCategory(createCategoryDTO);

            if (!result.IsSuccess)
            {
                return StatusCode(result.Errors[0].HttpStatusCode, result);
            }

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategoryById(int id)
        {
            var result = await _categoryService.GetCategoryById(id);

            if (!result.IsSuccess)
            {
                return StatusCode(result.Errors[0].HttpStatusCode, result);
            }

            return Ok(result);
        }
    }
}
