using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PricingCalculator.Dtos.Requests.Products;
using PricingCalculator.Services.ProductServices;

namespace PricingCalculator.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ProductsController(IProductService productService) : ControllerBase
    {
        private readonly IProductService _productService = productService;


        [HttpGet]
        public async Task<IActionResult> getAllProducts()
        {
            var result = await _productService.GetAllProducts();

            if (!result.IsSuccess)
            {
                return StatusCode(result.Errors[0].HttpStatusCode, result);
            }

            return Ok(result);
        }

        [HttpPost, Authorize(Roles = "Administrator")]
        public async Task<IActionResult> CreateProduct(CreateProductDTO createProductDTO)
        {
            var result = await _productService.CreateProduct(createProductDTO);

            if (!result.IsSuccess)
            {
                return StatusCode(result.Errors[0].HttpStatusCode, result);
            }

            return Ok(result);
        }
    }
}
