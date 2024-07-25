using System.ComponentModel.DataAnnotations;

namespace PricingCalculator.Models
{
    public class Category
    {
        public int Id { get; set; }
        [StringLength(100, ErrorMessage = "The field {0} has a max length of {1} characters ")]
        public string Name { get; set; } = null!;
        public virtual List<Product> Products { get; set; } = new List<Product>();
    }
}
