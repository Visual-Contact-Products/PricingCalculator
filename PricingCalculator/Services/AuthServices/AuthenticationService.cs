using Mapster;
using Microsoft.AspNetCore.Identity;
using PricingCalculator.Data.Repositories.RefreshTokenRepositories;
using PricingCalculator.Dtos.Requests.Authentication;
using PricingCalculator.Dtos.Responses;
using PricingCalculator.Dtos.Responses.Authentication;
using PricingCalculator.Helpers;
using PricingCalculator.Models;
using PricingCalculator.Models.Errors;
using PricingCalculator.Models.Results;
using PricingCalculator.Services.TokenServices;
using PricingCalculator.Services.UserServices;

namespace PricingCalculator.Services.AuthServices
{
    public class AuthenticationService(IUserService userService, SignInManager<PricingCalculator.Models.User> signInManager,
        IRefreshTokenRepository refreshTokenRepository,
        ITokenService tokenService, IGetUserInfo getUserInfo) : IAuthenticationService
    {
        private readonly IUserService _userService = userService;
        private readonly SignInManager<PricingCalculator.Models.User> _signInManager = signInManager;
        private readonly IRefreshTokenRepository _refreshTokenRepository = refreshTokenRepository;
        private readonly ITokenService _tokenService = tokenService;
        private readonly IGetUserInfo _getUserInfo = getUserInfo;

        public async Task<Result<LoginResponse, Error>> Login(LoginRequest request)
        {
            var user = await _userService.FindUserByEmail(request.Email);

            if (!user.IsSuccess || user.Value == null)
            {
                return Result<LoginResponse, Error>.Failure(user.Errors);
            }

            var signin = await _signInManager
                .CheckPasswordSignInAsync(user.Value, request.Password, lockoutOnFailure: false);

            if (!signin.Succeeded)
            {
                return Result<LoginResponse, Error>.Failure(Errors.PasswordDoesNotMatch);
            }

            var tokenProcesor = await _tokenService.ProcessTokenAsync(user.Value);

            var loginUserResponse = user.Value.BuildAdapter().AdaptToType<LoginUserResponse>();

            var response = new LoginResponse(tokenProcesor.Token,
                tokenProcesor.RefreshToken,
                loginUserResponse.Id,
                loginUserResponse.UserName,
                loginUserResponse.Email);

            return Result<LoginResponse, Error>.Success(response);
        }
        public async Task<Result<string, Error>> Logout()
        {
            string id = await _getUserInfo.GetUserId();

            if (!Guid.TryParse(id, out Guid userId))
            {
                return Result<string, Error>.Failure(Errors.UnauthorizedAccess);
            }

            await _refreshTokenRepository.DeleteAll(userId);
            await _signInManager.SignOutAsync();

            return Result<string, Error>.Success();
        }

        public async Task<Result<AuthenticatedUserResponse, Error>> CreateRefreshToken(RefreshRequest refreshRequest)
        {
            var isValidRefreshToken = _tokenService.Validate(refreshRequest.RefreshToken);

            if (!isValidRefreshToken)
            {
                return Result<AuthenticatedUserResponse, Error>.Failure(Errors.InvalidRefreshToken);
            }

            RefreshToken? refreshTokenDTO = await _refreshTokenRepository.GetByToken(refreshRequest.RefreshToken);

            if (refreshTokenDTO == null)
            {
                return Result<AuthenticatedUserResponse, Error>.Failure(Errors.RefreshTokenNotFound);
            }

            var user = await _userService.GetUserById(refreshTokenDTO.UserId);

            if (!user.IsSuccess || user.Value == null)
            {
                return Result<AuthenticatedUserResponse, Error>.Failure(user.Errors);
            }

            await _refreshTokenRepository.Delete(refreshTokenDTO.Id);

            AuthenticatedUserResponse response = await _tokenService.ProcessTokenAsync(user.Value);

            return Result<AuthenticatedUserResponse, Error>.Success(response);
        }
    }
}