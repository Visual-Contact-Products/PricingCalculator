using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PricingCalculator.Dtos.Requests.Authentication;
using PricingCalculator.Models.Errors;
using PricingCalculator.Models.Results;
using PricingCalculator.Services.AuthServices;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PricingCalculator.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController(IAuthenticationService authenticationService,
        IConfiguration configuration) : ControllerBase
    {
        private readonly IAuthenticationService _authenticationService = authenticationService;
        private readonly IConfiguration _configuration = configuration;


        // POST: api/authentication/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(Result<string, Error>.Failure(Errors.BadRequest));
            }

            var result = await _authenticationService.Login(loginRequest);

            if (!result.IsSuccess || string.IsNullOrEmpty(result.Value?.AccessToken))
            {
                return StatusCode(result.Errors[0].HttpStatusCode, result);
            }

            Response.Cookies.Append("Jwt", result.Value.AccessToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true, // Set to true if using HTTPS
                SameSite = SameSiteMode.Strict, // Adjust as needed
                Expires = DateTime.UtcNow.AddMinutes(double.Parse(_configuration.
                GetSection("Jwt:TokenExpirationInMinutes").Value!)) // Set expiration time
            });

            return Ok(result);
        }

        // POST: api/authentication/refresh-token
        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshTokenAsync([FromBody] RefreshRequest refreshRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(Result<string, Error>.Failure(Errors.BadRequest));
            }

            var response = await _authenticationService.CreateRefreshToken(refreshRequest);
            if (!response.IsSuccess)
            {
                return StatusCode(response.Errors[0].HttpStatusCode, response);
            }

            return Ok(response);
        }

        // POST: api/authentication/logout
        [HttpDelete("logout"), Authorize]
        public async Task<IActionResult> Logout()
        {
            var result = await _authenticationService.Logout();
            if (!result.IsSuccess)
            {
                return StatusCode(result.Errors[0].HttpStatusCode, result);
            }

            return Ok(Result<string, Error>.Success("You are logged out"));
        }

    }
}
