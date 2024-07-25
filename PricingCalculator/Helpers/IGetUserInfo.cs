using System.Security.Claims;

namespace PricingCalculator.Helpers
{
    public interface IGetUserInfo
    {
        Task<string> GetRole();
        public Task<string> GetUserId();
        public bool IsAdmin();
    }

    // Class that helps us to retrieve idUser anywhere in the application
    public class GetUserInfo : IGetUserInfo
    {
        private readonly IHttpContextAccessor httpContextAccessor;

        public GetUserInfo(IHttpContextAccessor httpContextAccessor)
        {
            this.httpContextAccessor = httpContextAccessor;
        }

        public async Task<string> GetUserId()
        {
            string idUser = httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            return idUser;
        }
        public async Task<string> GetRole()
        {
            string Role = httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Role);
            return Role;
        }
        public bool IsAdmin()
        {
            bool Role = httpContextAccessor.HttpContext.User.IsInRole("SuperAdministrator") ||
                httpContextAccessor.HttpContext.User.IsInRole("Administrator");
            return Role;
        }
    }
}
