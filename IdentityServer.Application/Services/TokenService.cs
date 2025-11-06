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
    private readonly ILogger<TokenService> _logger;
    public TokenService(IConfiguration configuration, IClientService clientService, 
        ILogger<TokenService> logger, IUserService userService)
    {
        _configuration = configuration;
        _clientService = clientService;
        _logger = logger;
        _userService = userService;
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

        // Generate refresh token for password grant
        var refreshToken = GenerateRefreshToken();

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

        // Validate refresh token (implement refresh token validation)
        // This is simplified - you should validate against stored refresh tokens

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
        // Implement refresh token logic
        // This should validate the refresh token against stored tokens
        // and generate a new access token
        throw new NotImplementedException("Refresh token functionality needs to be implemented");
    }

    public async Task<bool> RevokeTokenAsync(string token)
    {
        // Implement token revocation logic
        // This should invalidate the token in your token store
        throw new NotImplementedException("Token revocation functionality needs to be implemented");
    }
}