using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Web;
using System.Linq.Expressions;
using PricingCalculator.Helpers.Pagination;
using PricingCalculator.Data;
using PricingCalculator.Helpers;
using PricingCalculator.Models.Errors;
using PricingCalculator.Dtos.Requests.Users;
using PricingCalculator.Clients.Interfaces;
using System.Text.Json;
using Newtonsoft.Json;
using PricingCalculator.Models;
using PricingCalculator.Models.Results;

namespace PricingCalculator.Services.UserServices
{
    public class UserService(UserManager<PricingCalculator.Models.User> userManager,
        IEmailClient emailClient,
        IConfiguration configuration,
        IUnitOfWork unitOfWork,
        IGetUserInfo getUserInfo) : IUserService
    {
        private readonly UserManager<PricingCalculator.Models.User> _userManager = userManager;
        private readonly IConfiguration _configuration = configuration;
        private readonly IEmailClient _emailClient = emailClient;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IGetUserInfo _getUserInfo = getUserInfo;

        public async Task<Result<IEnumerable<PricingCalculator.Models.User>, Error>> GetAllUsers()
        {
            return await _userManager.Users
            .ToListAsync();
        }

        public async Task<PagedList<PricingCalculator.Models.User>> GetUsers(string? searchTerm,
            string? sortColumn,
            string? sortOrder,
            int page = 1,
            int pageSize = 10)
        {
            IQueryable<PricingCalculator.Models.User> usersQuery = _userManager.Users;

            //Validate if a filter exist for the query and apply it
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                usersQuery = usersQuery.Where(u =>
                u.UserName!.Contains(searchTerm) ||
                u.Email!.Contains(searchTerm)
                );
            }

            //Determine the column by which the query will be ordered - default value userId
            Expression<Func<PricingCalculator.Models.User, object>> keySelector = sortColumn?.ToLower() switch
            {
                "userName" => user => user.UserName,
                "email" => user => user.Email,
                _ => user => user.Id
            };

            //Order the query
            if (sortOrder?.ToLower() == "desc")
            {
                usersQuery = usersQuery.OrderByDescending(keySelector);
            }
            else
            {
                usersQuery = usersQuery.OrderBy(keySelector);
            }

            var users = await PagedList<PricingCalculator.Models.User>.CreateAsync(usersQuery, page, pageSize);

            return users;
        }

        public async Task<Result<PricingCalculator.Models.User, Error>> GetUserById(string id)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
            {
                return Result<PricingCalculator.Models.User, Error>.Failure(Errors.UserNotFound);
            }

            return Result<PricingCalculator.Models.User, Error>.Success(user);
        }

        public async Task<Result<string, Error>> UpdateUser(UpdateUserRequest updateUserRequest, PricingCalculator.Models.User user)
        {
            _ = updateUserRequest.ToModel(user);

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                // Map IdentityError to Error type
                var errors = result.Errors
                    .Select(error => new Error(error.Code, error.Description)).ToList();

                return Result<string, Error>.Failure(errors);
            }

            return Result<string, Error>.Success();
        }

        public async Task<Result<string, Error>> CreateUser(CreateUserRequest user)
        {
            await _unitOfWork.BeginTransactionAsync();

            var newUser = user.ToModel();

            // Generate a username, automatically, from the email address
            GenerateUsername(newUser);

            // Generate a random password for the user
            var randomPassword = GenerateRandomPassword();

            newUser.PasswordHash = _userManager.PasswordHasher.HashPassword(newUser, randomPassword);

            // Create the user in the database
            var result = await _userManager.CreateAsync(newUser);

            if (!result.Succeeded)
            {
                // Map IdentityError to Error type
                var errors = result.Errors.Select(error => new Error(error.Code, error.Description, 400)).ToList();
                await _unitOfWork.RollbackAsync();

                return Result<string, Error>.Failure(errors);
            }

            // Important: The user must be saved to the database before sending the email confirmation token
            // Otherwise, the token generated will always be invalid.

            // Send the user an email with their randomly generated password
            var isSendedPasswordEmail = await SendGeneratedPassword(newUser, randomPassword);

            if (!isSendedPasswordEmail.IsSuccess)
            {
                await _unitOfWork.RollbackAsync();
                return Result<string, Error>.Failure(Errors.EmailCanNotBeSended);
            }

            await _unitOfWork.CommitAsync();
            return Result<string, Error>.Success();
        }

        public async Task<Result<string, Error>> DeleteUser(PricingCalculator.Models.User user)
        {
            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                // Map IdentityError to Error type
                var errors = result.Errors.Select(error => new Error(error.Code, error.Description, 400)).ToList();

                return Result<string, Error>.Failure(errors);
            }

            return Result<string, Error>.Success();
        }

        public async Task<Result<PricingCalculator.Models.User, Error>> FindUserByEmail(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {

                return Result<PricingCalculator.Models.User, Error>.Failure(Errors.UserNotFound);
            }

            return Result<PricingCalculator.Models.User, Error>.Success(user);
        }

        public async Task<bool> ValidatePassword(PricingCalculator.Models.User user, string password)
        {
            return await _userManager.CheckPasswordAsync(user, password);
        }

        public async Task<Result<string, Error>> ChangePassword(string id, ChangePasswordRequest changePasswordRequest)
        {
            var user = await GetUserById(id);

            if (!user.IsSuccess || user.Value == null)
            {

                return Result<string, Error>.Failure(user.Errors);

            }

            var result = await _userManager
                .ChangePasswordAsync(user.Value, changePasswordRequest.CurrentPassword, changePasswordRequest.NewPassword);
            if (!result.Succeeded)
            {
                // Map IdentityError to Error type
                var errors = result.Errors.Select(error => new Error(error.Code, error.Description)).ToList();

                return Result<string, Error>.Failure(errors);
            }

            return Result<string, Error>.Success();
        }

        public async Task<Result<string, Error>> ForgotPassword(string email)
        {
            var user = await FindUserByEmail(email);

            if (!user.IsSuccess || user.Value == null)
            {
                return Result<string, Error>.Failure(user.Errors);
            }

            var result = await SendForgotPasswordEmail(user.Value);
            if (!result.IsSuccess)
            {
                return Result<string, Error>.Failure(result.Errors);
            }
            return result;
        }

        private static string GenerateRandomPassword(int length = 16)
        {
            const string validChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890!@#$%^&*()_+";

            var random = new Random();
            var passwordChars = new char[length];

            for (int i = 0; i < length; i++)
            {
                passwordChars[i] = validChars[random.Next(validChars.Length)];
            }

            return new string(passwordChars);
        }

        private static void GenerateUsername(PricingCalculator.Models.User user)
        {
            // If the user doesn't have a username, but does have an email, use the email prefix as the username
            if (string.IsNullOrEmpty(user.UserName) && !string.IsNullOrEmpty(user.Email))
            {
                var emailPrefix = user.Email.Split('@')[0];

                user.UserName = emailPrefix;
            }
        }

        private async Task<Result<string, Error>> SendGeneratedPassword(PricingCalculator.Models.User user, string generatedPassword)
        {
            if (user.Email == null)
            {
                return Result<string, Error>.Failure(Errors.EmailCanNotBeEmpty);
            }
            //build the property To since the service is expecting a string representing a list
            var emailAddress = new List<string> { user.Email };
            string toEmails = JsonConvert.SerializeObject(emailAddress);
            var message = new EmailMessage
            {
                From = "Pricing calculator",
                To = toEmails,
                Subject = "Welcome to Pricing Calculator",
                Content = $"<h1> Welcome {user.UserName} </h1> <p>Your password is: {generatedPassword}</p>"
            };

            // Call the email microservice to send the email
            await _emailClient.SendEmailAsync(message);

            return Result<string, Error>.Success();
        }

        private async Task<Result<string, Error>> SendForgotPasswordEmail(PricingCalculator.Models.User user)
        {
            // Generate a password reset token
            var resetPasswordToken = await _userManager.GeneratePasswordResetTokenAsync(user);

            var tokenEncoded = HttpUtility.UrlEncode(resetPasswordToken, Encoding.UTF8);

            if (user.Email == null)
            {

                return Result<string, Error>.Failure(Errors.EmailCanNotBeEmpty);
            }
            //build the property To since the service is expecting a string representing a list
            var emailAddress = new List<string> { user.Email };
            string toEmails = JsonConvert.SerializeObject(emailAddress);
            var message = new EmailMessage
            {
                From = "Visual Contact",
                To = toEmails,
                Subject = "Reset your password",
                Content = $"<h1> Welcome {user.UserName} </h1> <p>Please click <a href=" +
                $"\"{_configuration.GetSection("FrontendUrl:BaseUrl").Value!}/reset-password?userId={user.Id}&token={tokenEncoded}\">here</a> to reset your password.</p>"
            };

            try
            {
                await _emailClient.SendEmailAsync(message);
            }
            catch
            {
                return Result<string, Error>.Failure(Errors.EmailServiceNotAvailable);
            }


            return Result<string, Error>.Success();
        }

        public async Task<Result<string, Error>> ResetPassword(string userId, string token, ResetPasswordRequest resetPasswordRequest)
        {
            var user = await GetUserById(userId);
            if (!user.IsSuccess || user.Value == null)
            {
                return Result<string, Error>.Failure(user.Errors);

            }

            var result = await _userManager.ResetPasswordAsync(user.Value, token, resetPasswordRequest.Password);
            if (!result.Succeeded)
            {
                // Map IdentityError to Error type
                var errors = result.Errors.Select(error => new Error(error.Code, error.Description, 400)).ToList();

                return Result<string, Error>.Failure(errors);
            }

            return Result<string, Error>.Success();
        }
    }
}