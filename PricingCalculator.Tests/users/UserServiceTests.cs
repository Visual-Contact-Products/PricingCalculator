using Xunit;
using NSubstitute;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using PricingCalculator.Clients.Interfaces;
using PricingCalculator.Models;
using PricingCalculator.Data;
using PricingCalculator.Services.UserServices;
using Microsoft.EntityFrameworkCore;
using NSubstitute.ReturnsExtensions;
using PricingCalculator.Models.Errors;
using MockQueryable.NSubstitute;
using PricingCalculator.Dtos.Requests.Users;
using PricingCalculator.Models.Results;
using System.Text;
using System.Web;
using NSubstitute.ExceptionExtensions;
using PricingCalculator.Helpers;
using NSubstitute.Extensions;

namespace PricingCalculator.Tests.users
{
    public class UserServiceTests
    {
        private readonly UserManager<PricingCalculator.Models.User> _userManager;
        private readonly IConfiguration _configuration;
        private readonly IEmailClient _emailClient;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGetUserInfo _getUserInfo;

        private UserService CreateUserService()
        {
            return Substitute.ForPartsOf<UserService>(
            _userManager,
            _emailClient,
            _configuration,
            _unitOfWork,
            _getUserInfo
            );
        }

        public UserServiceTests()
        {
            _userManager = Substitute.For<UserManager<PricingCalculator.Models.User>>(
            Substitute.For<IUserStore<PricingCalculator.Models.User>>(),
            null, null, null, null, null, null, null, null
            );
            _configuration = Substitute.For<IConfiguration>();
            _emailClient = Substitute.For<IEmailClient>();
            _unitOfWork = Substitute.For<IUnitOfWork>();
            _getUserInfo = Substitute.For<IGetUserInfo>();
        }

        [Fact]
        public async Task GetUserById_UserDoesNotExist_ReturnsError()
        {
            // Arrange
            var userId = Guid.NewGuid().ToString();

            // build mock by extension
            var users = new List<PricingCalculator.Models.User>().AsQueryable().BuildMock();

            // setup the mock as Queryable for NSubstitute
            _userManager.Users.Returns(users);

            var userService = CreateUserService();

            // Act
            var result = await userService.GetUserById(userId);

            // Assert
            Assert.Equal(Errors.UserNotFound, result.Errors[0]);
        }

        [Fact]
        public async Task GetUserById_UserExists_ReturnsUserData()
        {
            // Arrange
            var userId = Guid.NewGuid().ToString();
            var user = new PricingCalculator.Models.User
            {
                Id = userId,
                Email = "test@example.com",
                UserName = "TestUser",
            };

            // build mock by extension
            var users = new List<PricingCalculator.Models.User>() { user }.AsQueryable().BuildMock();

            // setup the mock as Queryable for NSubstitute
            _userManager.Users.Returns(users);

            var userService = CreateUserService();

            // Act
            var result = await userService.GetUserById(userId);

            // Assert
            Assert.Equal(user, result.Value);
        }

        [Fact]
        public async Task CreateUser_UserCreationFails_ReturnsError()
        {
            // Arrange
            var createUserRequest = new CreateUserRequest
            {
                Email = "test@example.com",
            };

            var identityErrors = new List<IdentityError>
            {
                new IdentityError { Code = "DuplicateUserName", Description = "Username already taken." }
            };
            var identityResult = IdentityResult.Failed(identityErrors.ToArray());

            _userManager.CreateAsync(Arg.Any<PricingCalculator.Models.User>()).Returns(Task.FromResult(identityResult));

            _userManager.PasswordHasher = Substitute.For<IPasswordHasher<PricingCalculator.Models.User>>();
            string PasswordHash = "HashPassword";
            _userManager.PasswordHasher.HashPassword(Arg.Any<PricingCalculator.Models.User>(), Arg.Any<string>())
                .Returns(PasswordHash);

            var userService = CreateUserService();

            // Act
            var result = await userService.CreateUser(createUserRequest);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.NotNull(result.Errors);
            Assert.Contains(result.Errors, e => e.Code == "DuplicateUserName");
        }

        [Fact]
        public async Task CreateUser_SendEmailsFails_Error()
        {
            // Arrange
            var createUserRequest = new CreateUserRequest
            {
                Email = "test@example.com"
            };

            // Configure the UserManager to create the user correctly
            var identityResult = IdentityResult.Success;
            _userManager.CreateAsync(Arg.Any<PricingCalculator.Models.User>()).Returns(Task.FromResult(identityResult));

            // Configure the PasswordHasher to return a simulated hash
            _userManager.PasswordHasher = Substitute.For<IPasswordHasher<PricingCalculator.Models.User>>();
            string PasswordHash = "HashPassword";
            _userManager.PasswordHasher.HashPassword(Arg.Any<PricingCalculator.Models.User>(), Arg.Any<string>())
                .Returns(PasswordHash);

            // Configure role addition for success
            var addRolesResult = IdentityResult.Success;
            _userManager.AddToRolesAsync(Arg.Any<PricingCalculator.Models.User>(), Arg.Any<IEnumerable<string>>())
                .Returns(addRolesResult);

            // Set sending emails to fail
            _emailClient.SendEmailAsync(Arg.Any<EmailMessage>())
                .Throws(new Exception("Failed to send email"));

            var userService = CreateUserService();

            // Act
            var result = await userService.CreateUser(createUserRequest);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains(result.Errors, e => e.Code == Errors.EmailCanNotBeSended.Code);
        }

        [Fact]
        public async Task CreateUser_UserCreatedSuccessfully_ReturnsSuccessResult()
        {
            // Arrange
            var createUserRequest = new CreateUserRequest
            {
                Email = "test@example.com",
            };

            var identityResult = IdentityResult.Success;
            _userManager.CreateAsync(Arg.Any<PricingCalculator.Models.User>()).Returns(Task.FromResult(identityResult));

            _userManager.PasswordHasher = Substitute.For<IPasswordHasher<PricingCalculator.Models.User>>();
            string PasswordHash = "HashPassword";
            _userManager.PasswordHasher.HashPassword(Arg.Any<PricingCalculator.Models.User>(), Arg.Any<string>())
                .Returns(PasswordHash);

            var addRolesResult = IdentityResult.Success;
            _userManager.AddToRolesAsync(Arg.Any<PricingCalculator.Models.User>(), Arg.Any<IEnumerable<string>>())
                .Returns(addRolesResult);

            _emailClient.SendEmailAsync(Arg.Any<EmailMessage>()).Returns(Result<string, Error>.Success());

            var userService = CreateUserService();

            // Act
            var result = await userService.CreateUser(createUserRequest);

            // Assert
            Assert.True(result.IsSuccess);
        }
     
        [Fact]
        public async Task ChangePassword_UserNotFound_ReturnsError()
        {
            // Arrange
            var userId = Guid.NewGuid().ToString();
            var changePasswordRequest = new ChangePasswordRequest
            {
                CurrentPassword = "OldPassword123!",
                NewPassword = "NewPassword123!"
            };

            // build mock by extension
            var users = new List<PricingCalculator.Models.User>().AsQueryable().BuildMock();

            // setup the mock as Queryable for NSubstitute
            _userManager.Users.Returns(users);

            var userService = CreateUserService();

            // Act
            var result = await userService.ChangePassword(userId, changePasswordRequest);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains(result.Errors, e => e.Code == Errors.UserNotFound.Code);
        }

        [Fact]
        public async Task ChangePassword_FailureFromUserManager_ReturnsError()
        {
            // Arrange
            var userId = Guid.NewGuid().ToString();
            var changePasswordRequest = new ChangePasswordRequest
            {
                CurrentPassword = "OldPassword123!",
                NewPassword = "NewPassword123!"
            };

            var user = new PricingCalculator.Models.User { Id = userId, UserName = "TestUser" };

            var identityErrors = new List<IdentityError>
            {
                new IdentityError { Code = "Error1", Description = "Error 1 description" },
                new IdentityError { Code = "Error2", Description = "Error 2 description" }
            };

            var identityResult = IdentityResult.Failed(identityErrors.ToArray());

            _userManager.ChangePasswordAsync(user, changePasswordRequest.CurrentPassword, changePasswordRequest.NewPassword)
                .Returns(Task.FromResult(identityResult));

            // build mock by extension
            var users = new List<PricingCalculator.Models.User>() { user }.AsQueryable().BuildMock();

            // setup the mock as Queryable for NSubstitute
            _userManager.Users.Returns(users);

            var userService = CreateUserService();

            // Act
            var result = await userService.ChangePassword(userId, changePasswordRequest);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(identityErrors.Count, result.Errors.Count);
            Assert.Equal(identityErrors[0].Code, result.Errors[0].Code);
            Assert.Equal(identityErrors[0].Description, result.Errors[0].Description);
        }

        [Fact]
        public async Task ChangePassword_Success_ReturnsSuccessResult()
        {
            // Arrange
            var userId = "123";
            var changePasswordRequest = new ChangePasswordRequest
            {
                CurrentPassword = "OldPassword123!",
                NewPassword = "NewPassword123!"
            };

            var user = new PricingCalculator.Models.User { Id = userId, UserName = "TestUser" };

            // Configurar UserManager para que ChangePasswordAsync devuelva éxito
            var identityResult = IdentityResult.Success;
            _userManager.ChangePasswordAsync(user, changePasswordRequest.CurrentPassword, changePasswordRequest.NewPassword)
                .Returns(Task.FromResult(identityResult));

            // build mock by extension
            var users = new List<PricingCalculator.Models.User>() { user }.AsQueryable().BuildMock();

            // setup the mock as Queryable for NSubstitute
            _userManager.Users.Returns(users);

            var userService = CreateUserService();

            // Act
            var result = await userService.ChangePassword(userId, changePasswordRequest);

            // Assert
            Assert.True(result.IsSuccess);
        }

        [Fact]
        public async Task ResetPassword_UserNotFound_ReturnsError()
        {
            // Arrange
            var userId = Guid.NewGuid().ToString();

            // build mock by extension
            var users = new List<PricingCalculator.Models.User>().AsQueryable().BuildMock();

            // setup the mock as Queryable for NSubstitute
            _userManager.Users.Returns(users);

            var token = "dummy-token";
            var request = new ResetPasswordRequest
            {
                Password = "NewPassword123",
                ConfirmPassword = "NewPassword123"
            };

            var userService = CreateUserService();

            // Act
            var result = await userService.ResetPassword(userId, token, request);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains(result.Errors, e => e.Code == "UserNotFound");
        }

        [Fact]
        public async Task ResetPassword_ResetFails_ReturnsError()
        {
            // Arrange
            var userId = Guid.NewGuid().ToString();
            var user = new PricingCalculator.Models.User { Id = userId, UserName = "TestUser" };
            // build mock by extension
            var users = new List<PricingCalculator.Models.User>() { user }.AsQueryable().BuildMock();

            // setup the mock as Queryable for NSubstitute
            _userManager.Users.Returns(users);

            var token = "dummy-token";
            var request = new ResetPasswordRequest
            {
                Password = "NewPassword123",
                ConfirmPassword = "NewPassword123"
            };

            _userManager.ResetPasswordAsync(user, token, request.Password)
           .Returns(IdentityResult.Failed(new IdentityError { Code = "ResetFailed", Description = "Password reset failed" }));

            var userService = CreateUserService();

            // Act
            var result = await userService.ResetPassword(userId, token, request);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains(result.Errors, e => e.Code == "ResetFailed");
        }

        [Fact]
        public async Task ResetPassword_Success_ReturnsSuccessResult()
        {
            // Arrange
            var userId = Guid.NewGuid().ToString();
            var user = new PricingCalculator.Models.User { Id = userId, UserName = "TestUser" };
            // build mock by extension
            var users = new List<PricingCalculator.Models.User>() { user }.AsQueryable().BuildMock();

            // setup the mock as Queryable for NSubstitute
            _userManager.Users.Returns(users);

            var token = "dummy-token";
            var request = new ResetPasswordRequest
            {
                Password = "NewPassword123",
                ConfirmPassword = "NewPassword123"
            };

            _userManager.ResetPasswordAsync(user, token, request.Password)
            .Returns(IdentityResult.Success);

            var userService = CreateUserService();

            // Act
            var result = await userService.ResetPassword(userId, token, request);

            // Assert
            Assert.True(result.IsSuccess);
        }
    }
}
