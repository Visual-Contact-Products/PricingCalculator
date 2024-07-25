using PricingCalculator.Models;
using System.ComponentModel.DataAnnotations;

namespace PricingCalculator.Dtos.Requests.Categories
{
    public class CreateCategoryDTO : BaseDto<CreateCategoryDTO, Category>
    {
        [StringLength(100, ErrorMessage = "The field {0} has a max length of {1} characters ")]
        public string Name { get; set; } = null!;
    }
}
