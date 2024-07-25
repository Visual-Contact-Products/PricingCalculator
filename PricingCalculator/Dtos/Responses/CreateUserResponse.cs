using PricingCalculator.Dtos;
using PricingCalculator.Models;

namespace PricingCalculator.Dtos.Responses
{
    public class CreateUserResponse : BaseDto<CreateUserResponse, PricingCalculator.Models.User>
    {
        public string? Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
    }
}
