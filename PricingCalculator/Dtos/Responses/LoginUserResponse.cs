using PricingCalculator.Models;

namespace PricingCalculator.Dtos.Responses
{
    public class LoginUserResponse : BaseDto<LoginUserResponse, PricingCalculator.Models.User>
    {
        public string? Id { get; set; }
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public bool? IsFirstLogin { get; set; }
    }
}
