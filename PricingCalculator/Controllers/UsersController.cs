using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using PricingCalculator.Services.UserServices;
using PricingCalculator.Dtos.Requests.Users;

namespace PricingCalculator.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController(IUserService userService) : ControllerBase
    {
        private readonly IUserService _userService = userService;

        [HttpPost, Authorize(Roles = "Administrator")]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest createUserRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _userService.CreateUser(createUserRequest);

            if (!result.IsSuccess)
            {
                return StatusCode(result.Errors[0].HttpStatusCode, result);
            }
            return Ok(result);
        }

        // GET: api/users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PricingCalculator.Models.User>>> GetUsers()
        {
            return Ok(await _userService.GetAllUsers());
        }

        // GET: api/users/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(string id)
        {
            var user = await _userService.GetUserById(id);

            if (!user.IsSuccess || user.Value == null)
            {
                return StatusCode(user.Errors[0].HttpStatusCode, user);
            }

            return Ok(user);
        }

        // DELETE: api/users/5
        [HttpDelete("{id}"), Authorize(Roles = "Administrator")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await _userService.GetUserById(id);

            if (!user.IsSuccess || user.Value == null)
            {
                return StatusCode(user.Errors[0].HttpStatusCode, user);
            }

            var result = await _userService.DeleteUser(user.Value);

            if (!result.IsSuccess)
            {
                return StatusCode(result.Errors[0].HttpStatusCode, result);
            }

            return Ok(result);
        }

        // PATCH: api/users/{id}/change-password
        [HttpPatch("{id}/change-password")]
        public async Task<IActionResult> ChangePassword(string id, ChangePasswordRequest changePasswordRequest)
        {
            var result = await _userService.ChangePassword(id, changePasswordRequest);

            if (!result.IsSuccess)
            {
                return StatusCode(result.Errors[0].HttpStatusCode, result);
            }

            return Ok(result);
        }

        [HttpPost("forgot-password")]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword([FromBody] string email)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _userService.ForgotPassword(email);

            if (!result.IsSuccess)
            {
                return StatusCode(result.Errors[0].HttpStatusCode, result);
            }

            return Ok(result);

        }

        [HttpPatch("reset-password")]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword([FromQuery] string userId,
            [FromQuery] string token, [FromBody] ResetPasswordRequest resetPasswordRequest)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _userService.ResetPassword(userId, token, resetPasswordRequest);

            // Verify that the user exists
            if (!result.IsSuccess)
            {
                return StatusCode(result.Errors[0].HttpStatusCode, result);
            }

            return Ok(result);
        }
    }
}