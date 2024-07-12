using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PricingCalculator.Models
{
    public class Product
    {
        public Guid Id { get; set; }
        [StringLength(100, ErrorMessage = "The field {0} has a max length of {1} characters ")]
        public string Sku { get; set; } = null!;

        [StringLength(250, ErrorMessage = "The field {0} has a max length of {1} characters ")]
        public string Description { get; set; } = null!;
        public double Price { get; set; }
        public virtual int CategoryId { get; set; }

        [ForeignKey("CategoryId")]
        public virtual Category Category { get; set; }
    }
}
