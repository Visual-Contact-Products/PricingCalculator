namespace PricingCalculator.Dtos.Requests.Users
{
    public class ResetPasswordRequest
    {
        public string Password { get; set; } = null!;
        public string ConfirmPassword { get; set; } = null!;
    }
}
