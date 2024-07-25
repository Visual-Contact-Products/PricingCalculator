using System.ComponentModel.DataAnnotations.Schema;

namespace PricingCalculator.Models
{
    [NotMapped]
    public class EmailMessage
    {
        // Sender's name
        public required string From { get; set; }
        // Receivers email
        public required string To { get; set; }
        // Subject of the email
        public required string Subject { get; set; }
        // Content of the email
        public required string Content { get; set; }
    }
}
