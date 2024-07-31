using Microsoft.AspNetCore.Identity;

namespace PricingCalculator.Models
{
    public class User : IdentityUser
    {
        public bool? IsFirstLogin { get; set; } = true;
        public User()
        {
        }
    }
}
