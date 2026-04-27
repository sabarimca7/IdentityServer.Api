using IdentityServer.Domain.Entities;

namespace IdentityServer.Application.Interfaces;

public interface IRefreshTokenService
{
    Task<RefreshToken> CreateRefreshTokenAsync(RefreshToken refreshToken);
    Task<RefreshToken?> GetRefreshTokenByTokenAsync(string token);
    Task<bool> RevokeRefreshTokenAsync(string token);
    Task<bool> RevokeRefreshTokenByIdAsync(int refreshTokenId);
    Task<bool> DeleteExpiredTokensAsync();
}
