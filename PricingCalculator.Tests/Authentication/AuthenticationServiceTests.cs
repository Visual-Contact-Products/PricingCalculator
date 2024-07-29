using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using NSubstitute;
using PricingCalculator.Dtos.Requests.Authentication;
using PricingCalculator.Models.Results;
using PricingCalculator.Services.AuthServices;
using PricingCalculator.Services.UserServices;
using PricingCalculator.Models;
using PricingCalculator.Models.Errors;
using Xunit;
using PricingCalculator.Data.Repositories.RefreshTokenRepositories;
using Microsoft.Extensions.Configuration;
using PricingCalculator.Dtos.Responses;
using PricingCalculator.Services.TokenServices;
using PricingCalculator.Helpers;
using NSubstitute.ReturnsExtensions;

namespace PricingCalculator.Tests.Authentication
{
    public class AuthenticationServiceTests
    {
        private readonly IUserService _userService;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly SignInManager<PricingCalculator.Models.User> _signInManager;
        private readonly ITokenService _tokenService;
        private readonly IGetUserInfo _getUserInfo;

        public AuthenticationServiceTests()
        {
            _userService = Substitute.For<IUserService>();
            _refreshTokenRepository = Substitute.For<IRefreshTokenRepository>();
            _tokenService = Substitute.For<ITokenService>();
            _getUserInfo = Substitute.For<IGetUserInfo>();
            _signInManager = Substitute.For<SignInManager<PricingCalculator.Models.User>>(
            Substitute.For<UserManager<PricingCalculator.Models.User>>(
                Substitute.For<IUserStore<PricingCalculator.Models.User>>(),
                null, null, null, null, null, null, null, null),
            Substitute.For<IHttpContextAccessor>(),
            Substitute.For<IUserClaimsPrincipalFactory<PricingCalculator.Models.User>>(),
            null, null, null, null);
        }

        private AuthenticationService CreateAuthenticationService()
        {
            return new AuthenticationService(_userService, _signInManager, _refreshTokenRepository, _tokenService, _getUserInfo);
        }

        [Fact]
        public async Task Login_ValidCredentials_ReturnsSuccessResult()
        {
            // Arrange
            var request = new LoginRequest
            {
                Email = "test@example.com",
                Password = "password"
            };

            var user = new PricingCalculator.Models.User
            {
                Id = Guid.NewGuid().ToString(),
                Email = request.Email,
                UserName = "test",
                EmailConfirmed = true,
            };

            _userService.FindUserByEmail(request.Email).Returns(Result<PricingCalculator.Models.User, Error>.Success(user));

            _signInManager.CheckPasswordSignInAsync(user, request.Password, false)
                .Returns(Task.FromResult(SignInResult.Success));

            // Mocking the ITokenService
            _tokenService.ProcessTokenAsync(Arg.Any<PricingCalculator.Models.User>()).Returns(Task.FromResult(new AuthenticatedUserResponse
            {
                Token = "mockedToken",
                RefreshToken = "mockedRefreshToken"
            }));

            var guidMock = "9e279d0f-61a1-4fe5-a9f3-71a5380b57a9";
            _getUserInfo.GetUserId().Returns(guidMock);

            List<string> roles = new() { "SuperAdministrator" };
            _userService.GetRoles(user).Returns(Result<IEnumerable<string>, Error>.Success(roles));

            var authService = CreateAuthenticationService();

            // Act
            var result = await authService.Login(request);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
        }

        [Fact]
        public async Task Login_InvalidEmail_ReturnsErrorUserNotFound()
        {
            // Arrange
            var request = new LoginRequest
            {
                Email = "test@example.com",
                Password = "password"
            };

            var user = new PricingCalculator.Models.User
            {
                Id = Guid.NewGuid().ToString(),
                Email = request.Email,
                UserName = "test",
                EmailConfirmed = true
            };

            _userService.FindUserByEmail(request.Email).Returns(Result<PricingCalculator.Models.User, Error>.Failure(Errors.UserNotFound));

            _signInManager.CheckPasswordSignInAsync(user, request.Password, false)
                .Returns(Task.FromResult(SignInResult.Success));

            // Mocking the ITokenService
            _tokenService.ProcessTokenAsync(Arg.Any<PricingCalculator.Models.User>()).Returns(Task.FromResult(new AuthenticatedUserResponse
            {
                Token = "mockedToken",
                RefreshToken = "mockedRefreshToken"
            }));

            var guidMock = "9e279d0f-61a1-4fe5-a9f3-71a5380b57a9";
            _getUserInfo.GetUserId().Returns(guidMock);

            List<string> roles = new() { "SuperAdministrator" };
            _userService.GetRoles(user).Returns(Result<IEnumerable<string>, Error>.Success(roles));

            var authService = CreateAuthenticationService();

            // Act
            var result = await authService.Login(request);

            // Assert
            Assert.Equal(result.Errors[0], Errors.UserNotFound);
        }

        [Fact]
        public async Task Login_InvalidCredentials_ReturnsErrorPasswordDoesNotMatch()
        {
            // Arrange
            var request = new LoginRequest
            {
                Email = "test@example.com",
                Password = "password"
            };

            var user = new PricingCalculator.Models.User
            {
                Id = Guid.NewGuid().ToString(),
                Email = request.Email,
                UserName = "test",
                EmailConfirmed = true
            };

            _userService.FindUserByEmail(request.Email).Returns(Result<PricingCalculator.Models.User, Error>.Success(user));

            _signInManager.CheckPasswordSignInAsync(user, request.Password, false)
                .Returns(Task.FromResult(SignInResult.Failed));

            // Mocking the ITokenService
            _tokenService.ProcessTokenAsync(Arg.Any<PricingCalculator.Models.User>()).Returns(Task.FromResult(new AuthenticatedUserResponse
            {
                Token = "mockedToken",
                RefreshToken = "mockedRefreshToken"
            }));

            var guidMock = "9e279d0f-61a1-4fe5-a9f3-71a5380b57a9";
            _getUserInfo.GetUserId().Returns(guidMock);

            List<string> roles = new() { "SuperAdministrator" };
            _userService.GetRoles(user).Returns(Result<IEnumerable<string>, Error>.Success(roles));

            var authService = CreateAuthenticationService();

            // Act
            var result = await authService.Login(request);

            // Assert
            Assert.Equal(result.Errors[0], Errors.PasswordDoesNotMatch);
        }

        [Fact]
        public async Task CreateRefreshToken_InvalidRefreshToken_returnsError()
        {
            //Arrange
            var guidMock = "9e279d0f-61a1-4fe5-a9f3-71a5380b57a9";
            var refreshRequest = new RefreshRequest
            {
                RefreshToken = guidMock
            };
            _tokenService.Validate(refreshRequest.RefreshToken).Returns(false);
            var authService = CreateAuthenticationService();

            //Act
            var result = await authService.CreateRefreshToken(refreshRequest);

            // Assert
            Assert.Equal(Errors.InvalidRefreshToken, result.Errors[0]);
        }

        [Fact]
        public async Task CreateRefreshToken_RefreshTokenNotFound_returnsError()
        {
            //Arrange
            var guidMock = "9e279d0f-61a1-4fe5-a9f3-71a5380b57a9";
            var refreshRequest = new RefreshRequest
            {
                RefreshToken = guidMock
            };
            _tokenService.Validate(refreshRequest.RefreshToken).Returns(true);
            _refreshTokenRepository.GetByToken(refreshRequest.RefreshToken).ReturnsNull();
            var authService = CreateAuthenticationService();

            //Act
            var result = await authService.CreateRefreshToken(refreshRequest);

            // Assert
            Assert.Equal(Errors.RefreshTokenNotFound, result.Errors[0]);
        }

        [Fact]
        public async Task CreateRefreshToken_UserNotFound_returnsError()
        {
            //Arrange
            const string guidMock = "9e279d0f-61a1-4fe5-a9f3-71a5380b57a9";
            const string userIdStub = "22222222-2222-2222-2222-222222222222";
            var refreshRequest = new RefreshRequest
            {
                RefreshToken = guidMock
            };
            var refreshToken = new RefreshToken
            {
                Id = Guid.NewGuid(),
                Token = guidMock,
                UserId = userIdStub
            };
            _tokenService.Validate(refreshRequest.RefreshToken).Returns(true);
            _refreshTokenRepository.GetByToken(refreshRequest.RefreshToken).Returns(refreshToken);
            _userService.GetUserById(refreshToken.UserId).Returns(Result<PricingCalculator.Models.User, Error>.Failure(Errors.UserNotFound));
            var authService = CreateAuthenticationService();

            //Act
            var result = await authService.CreateRefreshToken(refreshRequest);

            // Assert
            Assert.Equal(Errors.UserNotFound, result.Errors[0]);
        }

        [Fact]
        public async Task CreateRefreshToken_ValidRefreshToken_returnsAuthenticatedUserResponse()
        {
            //Arrange
            const string guidMock = "9e279d0f-61a1-4fe5-a9f3-71a5380b57a9";
            const string userIdStub = "22222222-2222-2222-2222-222222222222";
            var refreshRequest = new RefreshRequest
            {
                RefreshToken = guidMock
            };
            var refreshToken = new RefreshToken
            {
                Id = Guid.NewGuid(),
                Token = guidMock,
                UserId = userIdStub
            };
            var user = new PricingCalculator.Models.User
            {
                Id = Guid.NewGuid().ToString(),
                Email = "test@gmail.com",
                UserName = "test",
                EmailConfirmed = true,
            };
            var authenticatedUserResponse = new AuthenticatedUserResponse
            {
                Token = Guid.NewGuid().ToString(),
                RefreshToken = Guid.NewGuid().ToString()
            };
            _tokenService.Validate(refreshRequest.RefreshToken).Returns(true);
            _refreshTokenRepository.GetByToken(refreshRequest.RefreshToken).Returns(refreshToken);
            _userService.GetUserById(refreshToken.UserId).Returns(Result<PricingCalculator.Models.User, Error>.Success(user));
            _refreshTokenRepository.Delete(refreshToken.Id).Returns(Task.CompletedTask);
            _tokenService.ProcessTokenAsync(user).Returns(authenticatedUserResponse);
            var authService = CreateAuthenticationService();

            //Act
            var result = await authService.CreateRefreshToken(refreshRequest);

            // Assert
            Assert.Equal(result.Value, authenticatedUserResponse);
        }

        [Fact]
        public async Task Logout_UserNotAuthenticated_ReturnsErrorUnauthorizedAccess()
        {
            //Arange
            _getUserInfo.GetUserId().ReturnsNull();
            var authService = CreateAuthenticationService();

            // Act
            var result = await authService.Logout();

            // Assert
            Assert.Equal(Errors.UnauthorizedAccess, result.Errors[0]);
        }

        [Fact]
        public async Task Logout_UserAuthenticated_ReturnsSuccessResult()
        {
            //Arange
            const string userId = "22222222-2222-2222-2222-222222222222";
            _getUserInfo.GetUserId().Returns(userId);
            _refreshTokenRepository.DeleteAll(Guid.Parse(userId)).Returns(Task.CompletedTask);
            _signInManager.SignOutAsync().Returns(Task.CompletedTask);
            var authService = CreateAuthenticationService();

            // Act
            var result = await authService.Logout();

            // Assert
            Assert.True(result.IsSuccess);
        }
    }
}
