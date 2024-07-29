using Microsoft.IdentityModel.Tokens;
using PricingCalculator.Data.Repositories.RefreshTokenRepositories;
using PricingCalculator.Dtos.Responses;
using PricingCalculator.Models;
using PricingCalculator.Services.TokenServices;
using PricingCalculator.Services.UserServices;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace User.Microservice.Services.TokenServices
{
    public class TokenService : ITokenService
    {
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IConfiguration _configuration;
        private readonly IUserService _userService;

        public TokenService(IRefreshTokenRepository refreshTokenRepository,
            IConfiguration configuration,
            IUserService userService)
        {
            _refreshTokenRepository = refreshTokenRepository;
            _configuration = configuration;
            _userService = userService;
        }

        public async Task<string> GenerateToken(PricingCalculator.Models.User user)
        {
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.Id),
                new(ClaimTypes.Name, user.UserName!),
                new(ClaimTypes.Email, user.Email!)
            };

            var roles = await _userService.GetRoles(user);
            if (roles.IsSuccess && roles.Value != null)
            {
                foreach (var role in roles.Value)
                {
                    claims.Add(new Claim(ClaimTypes.Role, role));
                }
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("Jwt:Key").Value!));

            var token = new JwtSecurityToken(
                issuer: _configuration.GetSection("Jwt:Issuer").Value,
                audience: _configuration.GetSection("Jwt:Audience").Value,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(double.Parse(_configuration.GetSection("Jwt:TokenExpirationInMinutes")
                    .Value!)),
                signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string GenerateRefreshToken()
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("Jwt:RefreshKey").Value!));

            var token = new JwtSecurityToken(
                issuer: _configuration.GetSection("Jwt:Issuer").Value,
                audience: _configuration.GetSection("Jwt:Audience").Value,
                expires: DateTime.UtcNow.AddMinutes(double.Parse(_configuration.GetSection("Jwt:RefreshTokenExpirationInMinutes")
                    .Value!)),
                signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        // Handles the token and refreshToken Generation and store the refresh token in memory
        public async Task<AuthenticatedUserResponse> ProcessTokenAsync(PricingCalculator.Models.User user)
        {
            var token = await GenerateToken(user);
            var refreshToken = GenerateRefreshToken();

            RefreshToken refreshTokenDto = new()
            {
                Token = refreshToken,
                UserId = user.Id
            };
            await _refreshTokenRepository.Create(refreshTokenDto);

            return new AuthenticatedUserResponse() { Token = token, RefreshToken = refreshToken };
        }

        public bool Validate(string refreshToken)
        {
            TokenValidationParameters validationParameters = new TokenValidationParameters
            {
                ValidateActor = true,
                ValidateIssuer = true,
                ValidateAudience = true,
                RequireExpirationTime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = _configuration.GetSection("Jwt:Issuer").Value,
                ValidAudience = _configuration.GetSection("Jwt:Audience").Value,
                IssuerSigningKey =
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                _configuration.GetSection("Jwt:RefreshKey").Value!)
            ),
                ClockSkew = TimeSpan.Zero
            };
            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();

            try
            {
                tokenHandler.ValidateToken(refreshToken, validationParameters, out SecurityToken validateToken);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
