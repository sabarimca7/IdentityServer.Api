using IdentityServer.Application.Interfaces;
using IdentityServer.Common.Constants;
using IdentityServer.Common.Exceptions;
using IdentityServer.Common.Models;
using IdentityServer.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace IdentityServer.Application.Services;

public class TokenService : ITokenService
{
    private readonly IConfiguration _configuration;
    private readonly IClientService _clientService;
    private readonly IUserService _userService;
    private readonly IRefreshTokenService _refreshTokenService;
    private readonly IAuthorizationCodeService _authorizationCodeService;
    private readonly ILogger<TokenService> _logger;

    public TokenService(IConfiguration configuration, IClientService clientService, 
        ILogger<TokenService> logger, IUserService userService, IRefreshTokenService refreshTokenService, IAuthorizationCodeService authorizationCodeService)
    {
        _configuration = configuration;
        _clientService = clientService;
        _logger = logger;
        _userService = userService;
        _refreshTokenService = refreshTokenService;
        _authorizationCodeService = authorizationCodeService;
    }

    public async Task<TokenResponse> GenerateTokenAsync(TokenRequest request)
    {
        try
        {
            // Validate client credentials
            if (!await _clientService.ValidateClientCredentialsAsync(request.ClientId, request.ClientSecret))
            {
                throw new UnauthorizedException("Invalid client credentials");
            }

            var client = await _clientService.GetClientByClientIdAsync(request.ClientId);
            if (client == null || !client.IsActive)
            {
                throw new UnauthorizedException("Client not found or inactive");
            }

            var claims = new List<Claim>
            {
                new(AuthConstants.ClaimTypes.ClientId, request.ClientId),
                new(AuthConstants.ClaimTypes.Iss, _configuration["Jwt:Issuer"] ?? "IdentityServer"),
                new(AuthConstants.ClaimTypes.Aud, _configuration["Jwt:Audience"] ?? "IdentityServerAPI")
            };

            // Handle different grant types
            switch (request.GrantType)
            {
                case AuthConstants.GrantTypes.ClientCredentials:
                    return await HandleClientCredentialsGrant(request, client, claims);
                case AuthConstants.GrantTypes.Password:
                    return await HandlePasswordGrant(request, client, claims);
                case AuthConstants.GrantTypes.RefreshToken:
                    return await HandleRefreshTokenGrant(request, client, claims);
                case AuthConstants.GrantTypes.AuthorizationCode:
                    return await HandleAuthorizationCodeGrant(request, client, claims);
                default:
                    throw new InvalidGrantException($"Unsupported grant type: {request.GrantType}");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating token for client {ClientId}", request.ClientId);
            throw;
        }
    }

    private async Task<TokenResponse> HandleClientCredentialsGrant(TokenRequest request, Client client, List<Claim> claims)
    {
        var tokenExpiry = client.AccessTokenValidity ?? 3600; // Default 1 hour
        var token = GenerateJwtToken(claims, tokenExpiry);


        return new TokenResponse
        {
            AccessToken = token,
            TokenType = "Bearer",
            ExpiresIn = tokenExpiry,
            Scope = request.Scope
        };
    }

    private async Task<TokenResponse> HandlePasswordGrant(TokenRequest request, Client client, List<Claim> claims)
    {
        // Validate user credentials (implement user validation logic)
        // This is a simplified version
        if (string.IsNullOrEmpty(request.Username) || string.IsNullOrEmpty(request.Password))
        {
            throw new InvalidGrantException("Username and password are required");
        }
        if(!await _userService.ValidateUserCredentialsAsync(request.Username,request.Password))
        {
            throw new UnauthorizedException("Invalid username or password");
        }
        var user = await _userService.GetUserByUsernameAsync(request.Username);
        // Check if account is locked
        if (user.LockoutEnd.HasValue && user.LockoutEnd.Value > DateTime.UtcNow)
        {
            throw new UnauthorizedException("Account is temporarily locked");
        }
        if (!request.Password.Equals(user.Password))
        {
            // Increment failed login count
            await _userService.IncrementAccessFailedCountAsync(user.UserId);
            if (user.AccessFailedCount >= 5)
            {
                DateTime now = DateTime.UtcNow;
                DateTime future = now.AddMinutes(15);
                TimeSpan span = future - now;
                await _userService.LockUserAsync(user.UserId, span);
            }
            throw new UnauthorizedException("Invalid username or password");
        }
        await _userService.ResetAccessFailedCountAsync(user.UserId);
        // Add user-specific claims
        claims.Add(new Claim(AuthConstants.ClaimTypes.Sub, request.Username));
        claims.Add(new Claim(AuthConstants.ClaimTypes.UserId, request.Username));
        claims.Add(new Claim(AuthConstants.ClaimTypes.ClientId, request.ClientId));
        claims.Add(new Claim(AuthConstants.ClaimTypes.Scope, "payingguest_api"));

        var tokenExpiry = client.AccessTokenValidity ?? 3600;
        var token = GenerateJwtToken(claims, tokenExpiry);

        // Generate and store refresh token for password grant
        var refreshToken = GenerateRefreshToken();
        var refreshTokenEntity = new RefreshToken
        {
            Token = refreshToken,
            UserId = user.UserId,
            OAuthClientId = client.OAuthClientId,
            ExpiresOn = DateTime.UtcNow.AddDays(7), // Refresh token valid for 7 days
            IsRevoked = false,
            CreatedOn = DateTime.UtcNow.ToString("yyyy-MM-dd")
        };
        await _refreshTokenService.CreateRefreshTokenAsync(refreshTokenEntity);

        return new TokenResponse
        {
            AccessToken = token,
            TokenType = "Bearer",
            ExpiresIn = tokenExpiry,
            RefreshToken = refreshToken,
            Scope = request.Scope
        };
    }

    private async Task<TokenResponse> HandleRefreshTokenGrant(TokenRequest request, Client client, List<Claim> claims)
    {
        if (string.IsNullOrEmpty(request.RefreshToken))
        {
            throw new InvalidGrantException("Refresh token is required");
        }

        // Validate refresh token
        var refreshToken = await _refreshTokenService.GetRefreshTokenByTokenAsync(request.RefreshToken);
        if (refreshToken == null)
        {
            throw new UnauthorizedException("Invalid refresh token");
        }

        // Check if token is revoked
        if (refreshToken.IsRevoked == true)
        {
            throw new UnauthorizedException("Refresh token has been revoked");
        }

        // Check if token has expired
        if (refreshToken.ExpiresOn.HasValue && refreshToken.ExpiresOn < DateTime.UtcNow)
        {
            throw new UnauthorizedException("Refresh token has expired");
        }

        // Get user information if refresh token is tied to a user
        if (refreshToken.UserId.HasValue)
        {
            var user = await _userService.GetUserByIdAsync(refreshToken.UserId.Value);
            if (user == null || !user.IsActive)
            {
                throw new UnauthorizedException("User not found or inactive");
            }

            claims.Add(new Claim(AuthConstants.ClaimTypes.Sub, user.Username));
            claims.Add(new Claim(AuthConstants.ClaimTypes.UserId, user.UserId.ToString()));
        }

        var tokenExpiry = client.AccessTokenValidity ?? 3600;
        var token = GenerateJwtToken(claims, tokenExpiry);

        return new TokenResponse
        {
            AccessToken = token,
            TokenType = "Bearer",
            ExpiresIn = tokenExpiry,
            RefreshToken = request.RefreshToken, // Reuse or generate new
            Scope = request.Scope
        };
    }

    private async Task<TokenResponse> HandleAuthorizationCodeGrant(TokenRequest request, Client client, List<Claim> claims)
    {
        // Validate authorization code from store
        var authCode = await _authorizationCodeService.GetByCodeAsync(request.Code ?? string.Empty);
        if (authCode == null)
            throw new InvalidGrantException("Invalid authorization code");

        if (authCode.IsUsed)
            throw new InvalidGrantException("Authorization code has already been used");

        if (authCode.ExpiresOn < DateTime.UtcNow)
            throw new InvalidGrantException("Authorization code has expired");

        if (authCode.OAuthClientId.HasValue && authCode.OAuthClientId != client.OAuthClientId)
            throw new InvalidGrantException("Authorization code was not issued to this client");

        // If code tied to a user, fetch and add user claims
        if (authCode.UserId.HasValue)
        {
            var user = await _userService.GetUserByIdAsync(authCode.UserId.Value);
            if (user == null || !user.IsActive)
                throw new UnauthorizedException("User not found or inactive");

            claims.Add(new Claim(AuthConstants.ClaimTypes.Sub, user.Username));
            claims.Add(new Claim(AuthConstants.ClaimTypes.UserId, user.UserId.ToString()));

            // If offline_access requested, generate and persist refresh token tied to user
            if (!string.IsNullOrEmpty(request.Scope) && request.Scope.Contains(AuthConstants.Scopes.OfflineAccess))
            {
                var refreshTokenValue = GenerateRefreshToken();
                var refreshTokenEntity = new RefreshToken
                {
                    Token = refreshTokenValue,
                    UserId = user.UserId,
                    OAuthClientId = client.OAuthClientId,
                    ExpiresOn = DateTime.UtcNow.AddDays(30),
                    IsRevoked = false,
                    CreatedOn = DateTime.UtcNow.ToString("yyyy-MM-dd")
                };
                await _refreshTokenService.CreateRefreshTokenAsync(refreshTokenEntity);

                // Mark auth code as used
                await _authorizationCodeService.MarkAsUsedAsync(authCode.Code);

                var accessTokenValue = GenerateJwtToken(claims, client.AccessTokenValidity ?? 3600);
                return new TokenResponse
                {
                    AccessToken = accessTokenValue,
                    TokenType = "Bearer",
                    ExpiresIn = client.AccessTokenValidity ?? 3600,
                    RefreshToken = refreshTokenValue,
                    Scope = request.Scope
                };
            }
        }

        // If not issuing refresh token, simply mark code used and return access token
        await _authorizationCodeService.MarkAsUsedAsync(authCode.Code);

        var tokenExpiry = client.AccessTokenValidity ?? 3600;
        var accessToken = GenerateJwtToken(claims, tokenExpiry);

        return new TokenResponse
        {
            AccessToken = accessToken,
            TokenType = "Bearer",
            ExpiresIn = tokenExpiry,
            RefreshToken = null,
            Scope = request.Scope
        };
    }

    private string GenerateJwtToken(List<Claim> claims, int expiryInSeconds)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"] ?? "DefaultSecretKey"));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddSeconds(expiryInSeconds),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private string GenerateRefreshToken()
    {
        return Convert.ToBase64String(Guid.NewGuid().ToByteArray());
    }

    public async Task<bool> ValidateTokenAsync(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"] ?? "DefaultSecretKey");

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = _configuration["Jwt:Issuer"],
                ValidateAudience = true,
                ValidAudience = _configuration["Jwt:Audience"],
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };

            await tokenHandler.ValidateTokenAsync(token, validationParameters);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<TokenResponse> RefreshTokenAsync(string refreshToken)
    {
        try
        {
            if (string.IsNullOrEmpty(refreshToken))
            {
                throw new InvalidGrantException("Refresh token is required");
            }

            // Retrieve refresh token from database
            var storedRefreshToken = await _refreshTokenService.GetRefreshTokenByTokenAsync(refreshToken);
            if (storedRefreshToken == null)
            {
                throw new UnauthorizedException("Invalid refresh token");
            }

            // Check if token is revoked
            if (storedRefreshToken.IsRevoked == true)
            {
                throw new UnauthorizedException("Refresh token has been revoked");
            }

            // Check if token has expired
            if (storedRefreshToken.ExpiresOn.HasValue && storedRefreshToken.ExpiresOn < DateTime.UtcNow)
            {
                throw new UnauthorizedException("Refresh token has expired");
            }

            // Get client information
            var client = await _clientService.GetClientByIdAsync(storedRefreshToken.OAuthClientId ?? 0);
            if (client == null || !client.IsActive)
            {
                throw new UnauthorizedException("Client not found or inactive");
            }

            var claims = new List<Claim>
            {
                new(AuthConstants.ClaimTypes.ClientId, client.ClientId),
                new(AuthConstants.ClaimTypes.Iss, _configuration["Jwt:Issuer"] ?? "IdentityServer"),
                new(AuthConstants.ClaimTypes.Aud, _configuration["Jwt:Audience"] ?? "IdentityServerAPI")
            };

            // Add user claims if refresh token is tied to a user
            if (storedRefreshToken.UserId.HasValue)
            {
                var user = await _userService.GetUserByIdAsync(storedRefreshToken.UserId.Value);
                if (user == null || !user.IsActive)
                {
                    throw new UnauthorizedException("User not found or inactive");
                }

                claims.Add(new Claim(AuthConstants.ClaimTypes.Sub, user.Username));
                claims.Add(new Claim(AuthConstants.ClaimTypes.UserId, user.UserId.ToString()));
                claims.Add(new Claim(AuthConstants.ClaimTypes.Scope, "payingguest_api"));
            }

            var tokenExpiry = client.AccessTokenValidity ?? 3600;
            var newAccessToken = GenerateJwtToken(claims, tokenExpiry);

            _logger.LogInformation("Refresh token used successfully for client {ClientId}", client.ClientId);

            return new TokenResponse
            {
                AccessToken = newAccessToken,
                TokenType = "Bearer",
                ExpiresIn = tokenExpiry,
                RefreshToken = refreshToken, // Reuse the same refresh token
                Scope = string.Empty
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error refreshing token");
            throw;
        }
    }

    public async Task<bool> RevokeTokenAsync(string token)
    {
        try
        {
            if (string.IsNullOrEmpty(token))
            {
                throw new InvalidGrantException("Token is required");
            }

            // Try to revoke as refresh token first
            var isRefreshTokenRevoked = await _refreshTokenService.RevokeRefreshTokenAsync(token);

            if (isRefreshTokenRevoked)
            {
                _logger.LogInformation("Refresh token revoked successfully");
                return true;
            }

            // For access tokens (JWT), we would typically add them to a blacklist
            // For now, we'll just validate if it's a valid token and log the revocation attempt
            var isValidToken = await ValidateTokenAsync(token);
            if (isValidToken)
            {
                // In a real implementation, you would add this token to a blacklist/cache
                // that your ValidateTokenAsync method checks against
                _logger.LogInformation("Access token revocation recorded (would be added to blacklist in production)");
                return true;
            }

            _logger.LogWarning("Token revocation attempted on invalid token");
            throw new UnauthorizedException("Invalid token");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error revoking token");
            throw;
        }
    }
}