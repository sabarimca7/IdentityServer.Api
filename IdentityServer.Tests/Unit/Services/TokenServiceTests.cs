using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using FluentAssertions;
using IdentityServer.Application.Services;
using IdentityServer.Application.Interfaces;
using IdentityServer.Common.Models;
using IdentityServer.Common.Constants;
using IdentityServer.Common.Exceptions;
using IdentityServer.Domain.Entities;

namespace IdentityServer.Tests.Unit.Services;

public class TokenServiceTests
{
    private readonly Mock<IConfiguration> _configurationMock;
    private readonly Mock<IClientService> _clientServiceMock;
    private readonly Mock<ILogger<TokenService>> _loggerMock;
    private readonly TokenService _tokenService;

    public TokenServiceTests()
    {
        _configurationMock = new Mock<IConfiguration>();
        _clientServiceMock = new Mock<IClientService>();
        _loggerMock = new Mock<ILogger<TokenService>>();

        // Setup configuration
        _configurationMock.Setup(x => x["Jwt:Key"]).Returns("MySecretKeyForJwtTokenGeneration123456789");
        _configurationMock.Setup(x => x["Jwt:Issuer"]).Returns("IdentityServer");
        _configurationMock.Setup(x => x["Jwt:Audience"]).Returns("IdentityServerAPI");

        _tokenService = new TokenService(_configurationMock.Object, _clientServiceMock.Object, _loggerMock.Object,null);
    }

    [Fact]
    public async Task GenerateTokenAsync_ValidClientCredentials_ReturnsTokenResponse()
    {
        // Arrange
        var request = new TokenRequest
        {
            GrantType = AuthConstants.GrantTypes.ClientCredentials,
            ClientId = "test_client",
            ClientSecret = "test_secret",
            Scope = "read write"
        };

        var client = new Client
        {
            OAuthClientId = 1,
            ClientId = "test_client",
            IsActive = true,
            AccessTokenValidity = 3600
        };

        _clientServiceMock.Setup(x => x.ValidateClientCredentialsAsync(request.ClientId, request.ClientSecret))
            .ReturnsAsync(true);
        _clientServiceMock.Setup(x => x.GetClientByClientIdAsync(request.ClientId))
            .ReturnsAsync(client);

        // Act
        var result = await _tokenService.GenerateTokenAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.AccessToken.Should().NotBeEmpty();
        result.TokenType.Should().Be("Bearer");
        result.ExpiresIn.Should().Be(3600);
        result.Scope.Should().Be(request.Scope);
    }

    [Fact]
    public async Task GenerateTokenAsync_InvalidClientCredentials_ThrowsUnauthorizedException()
    {
        // Arrange
        var request = new TokenRequest
        {
            GrantType = AuthConstants.GrantTypes.ClientCredentials,
            ClientId = "invalid_client",
            ClientSecret = "invalid_secret"
        };

        _clientServiceMock.Setup(x => x.ValidateClientCredentialsAsync(request.ClientId, request.ClientSecret))
            .ReturnsAsync(false);

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedException>(() => _tokenService.GenerateTokenAsync(request));
    }

    [Fact]
    public async Task ValidateTokenAsync_ValidToken_ReturnsTrue()
    {
        // Arrange
        var request = new TokenRequest
        {
            GrantType = AuthConstants.GrantTypes.ClientCredentials,
            ClientId = "test_client",
            ClientSecret = "test_secret"
        };

        var client = new Client
        {
            OAuthClientId = 1,
            ClientId = "test_client",
            IsActive = true,
            AccessTokenValidity = 3600
        };

        _clientServiceMock.Setup(x => x.ValidateClientCredentialsAsync(request.ClientId, request.ClientSecret))
            .ReturnsAsync(true);
        _clientServiceMock.Setup(x => x.GetClientByClientIdAsync(request.ClientId))
            .ReturnsAsync(client);

        var tokenResponse = await _tokenService.GenerateTokenAsync(request);

        // Act
        var result = await _tokenService.ValidateTokenAsync(tokenResponse.AccessToken);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task ValidateTokenAsync_InvalidToken_ReturnsFalse()
    {
        // Arrange
        var invalidToken = "invalid.jwt.token";

        // Act
        var result = await _tokenService.ValidateTokenAsync(invalidToken);

        // Assert
        result.Should().BeFalse();
    }
}