using PricingCalculator.Models;
using System.ComponentModel.DataAnnotations;

namespace PricingCalculator.Dtos.Requests.Products
{
    public class CreateProductDTO : BaseDto<CreateProductDTO, Product>
    {
        [StringLength(100, ErrorMessage = "The field {0} has a max length of {1} characters ")]
        public string Sku { get; set; } = null!;

        [StringLength(250, ErrorMessage = "The field {0} has a max length of {1} characters ")]
        public string Description { get; set; } = null!;

        [Required]
        public double Price { get; set; }

        [Required]
        public int CategoryId { get; set; }
    }
}
