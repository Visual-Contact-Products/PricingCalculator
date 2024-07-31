using Microsoft.AspNetCore.Identity;

namespace PricingCalculator.Dtos.Responses.Authentication
{
    public class LoginResponse
    {
        public string? AccessToken { get; set; }
        public string? RefreshToken { get; set; }
        public string? Id { get; set; }
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public bool? IsFirstLogin { get; set; }
        public LoginResponse()
        {
        }

        public LoginResponse(string? accessToken,
                             string? refreshToken,
                             string? id,
                             string? userName,
                             string? email,
                             bool? isFirstLogin)
        {
            AccessToken = accessToken;
            RefreshToken = refreshToken;
            Id = id;
            UserName = userName;
            Email = email;
            IsFirstLogin = isFirstLogin;
        }
    }
}