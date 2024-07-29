using Microsoft.AspNetCore.Identity;
using PricingCalculator.Dtos.Requests.Users;
using PricingCalculator.Helpers.Pagination;
using PricingCalculator.Models.Errors;
using PricingCalculator.Models.Results;

namespace PricingCalculator.Services.UserServices
{
    public interface IUserService
    {
        Task<Result<IEnumerable<PricingCalculator.Models.User>, Error>> GetAllUsers();
        Task<Result<PricingCalculator.Models.User, Error>> GetUserById(string id);
        Task<Result<string, Error>> UpdateUser(UpdateUserRequest updateUserRequest, PricingCalculator.Models.User user);
        Task<Result<string, Error>> CreateUser(CreateUserRequest user);
        Task<Result<string, Error>> DeleteUser(PricingCalculator.Models.User user);
        Task<Result<PricingCalculator.Models.User, Error>> FindUserByEmail(string email);
        Task<bool> ValidatePassword(PricingCalculator.Models.User user, string password);
        Task<Result<string, Error>> ChangePassword(string id, ChangePasswordRequest changePasswordRequest);
        Task<Result<string, Error>> ForgotPassword(string email);
        Task<Result<string, Error>> ResetPassword(string userId, string token, ResetPasswordRequest resetPasswordRequest);
        Task<Result<IEnumerable<string>, Error>> GetRoles(PricingCalculator.Models.User user);
    }
}
