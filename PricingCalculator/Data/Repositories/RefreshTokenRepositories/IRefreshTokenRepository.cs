using PricingCalculator.Models;

namespace PricingCalculator.Data.Repositories.RefreshTokenRepositories
{
    public interface IRefreshTokenRepository
    {
        Task Create(RefreshToken refreshToken);

        Task<RefreshToken?> GetByToken(string token);

        Task Delete(Guid id);

        Task DeleteAll(Guid userId);
    }
}