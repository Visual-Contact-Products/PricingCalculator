namespace PricingCalculator.Dtos.Responses
{
    public class AuthenticatedUserResponse
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
    }
}
