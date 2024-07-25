

using PricingCalculator.Dtos.Responses;

namespace PricingCalculator.Services.TokenServices
{
    public interface ITokenService
    {
        Task<string> GenerateToken(PricingCalculator.Models.User user);
        string GenerateRefreshToken();
        Task<AuthenticatedUserResponse> ProcessTokenAsync(PricingCalculator.Models.User user);
        bool Validate(string refreshToken);
    }
}
