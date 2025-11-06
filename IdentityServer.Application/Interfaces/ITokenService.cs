using IdentityServer.Common.Models;

namespace IdentityServer.Application.Interfaces;

public interface ITokenService
{
    Task<TokenResponse> GenerateTokenAsync(TokenRequest request);
    Task<bool> ValidateTokenAsync(string token);
    Task<TokenResponse> RefreshTokenAsync(string refreshToken);
    Task<bool> RevokeTokenAsync(string token);
}