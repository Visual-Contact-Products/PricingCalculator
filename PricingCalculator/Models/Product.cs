using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace PricingCalculator.Models
{
    [Index("Sku", IsUnique = true, Name = "IX_Products_Sku")]
    public class Product
    {
        public Guid Id { get; set; }

        [StringLength(100, ErrorMessage = "The field {0} has a max length of {1} characters ")]
        public string Sku { get; set; } = null!;

        [StringLength(250, ErrorMessage = "The field {0} has a max length of {1} characters ")]
        public string Description { get; set; } = null!;
        public double Price { get; set; }
        public bool IsActive { get; set; } = true;
        public virtual int CategoryId { get; set; }

        [ForeignKey("CategoryId")]
        [JsonIgnore]
        public virtual Category Category { get; set; }
    }
}
