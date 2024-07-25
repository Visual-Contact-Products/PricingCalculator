using Microsoft.AspNetCore.Identity;
using PricingCalculator.Dtos.Requests.Authentication;
using PricingCalculator.Dtos.Responses;
using PricingCalculator.Dtos.Responses.Authentication;
using PricingCalculator.Models.Errors;
using PricingCalculator.Models.Results;

namespace PricingCalculator.Services.AuthServices
{
    public interface IAuthenticationService
    {
        Task<Result<LoginResponse, Error>> Login(LoginRequest request);
        Task<Result<string, Error>> Logout();
        Task<Result<AuthenticatedUserResponse, Error>> CreateRefreshToken(RefreshRequest refreshRequest);
    }
}
