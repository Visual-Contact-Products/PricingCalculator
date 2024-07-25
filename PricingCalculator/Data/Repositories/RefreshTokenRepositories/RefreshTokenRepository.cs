﻿using PricingCalculator.Models;

namespace PricingCalculator.Data.Repositories.RefreshTokenRepositories
{
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly List<RefreshToken> _refreshTokens = new List<RefreshToken>();
        public Task Create(RefreshToken refreshToken)
        {
            refreshToken.Id = Guid.NewGuid();

            _refreshTokens.Add(refreshToken);

            return Task.CompletedTask;
        }

        public Task Delete(Guid id)
        {
            _refreshTokens.RemoveAll(r => r.Id == id);
            return Task.CompletedTask;
        }

        public Task DeleteAll(Guid userId)
        {
            _refreshTokens.RemoveAll(r => r.UserId == userId.ToString());
            return Task.CompletedTask;
        }

        public Task<RefreshToken?> GetByToken(string token)
        {
            RefreshToken? refreshToken = _refreshTokens.FirstOrDefault(r => r.Token == token);

            return Task.FromResult(refreshToken);
        }
    }
}
