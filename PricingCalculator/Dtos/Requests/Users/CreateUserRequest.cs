using PricingCalculator.Models;
using System.ComponentModel.DataAnnotations;

namespace PricingCalculator.Dtos.Requests.Users
{
    public class CreateUserRequest : BaseDto<CreateUserRequest, PricingCalculator.Models.User>
    {
        [Required]
        [EmailAddress]
        public string? Email { get; set; }

        public CreateUserRequest() { }
        public CreateUserRequest(string email)
        {
            Email = email;
        }
    }
}
