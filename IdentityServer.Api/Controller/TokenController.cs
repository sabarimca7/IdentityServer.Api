using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using IdentityServer.Application.Interfaces;
using IdentityServer.Common.Models;

namespace IdentityServer.Api.Controllers;

/// <summary>
/// OAuth 2.0 Token Management Controller
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class TokenController : ControllerBase
{
    private readonly ITokenService _tokenService;
    private readonly ILogger<TokenController> _logger;

    /// <summary>
    /// Initializes a new instance of the TokenController
    /// </summary>
    /// <param name="tokenService">Token service for token operations</param>
    /// <param name="logger">Logger instance</param>
    public TokenController(ITokenService tokenService, ILogger<TokenController> logger)
    {
        _tokenService = tokenService;
        _logger = logger;
    }

    /// <summary>
    /// Generate OAuth 2.0 access token
    /// </summary>
    /// <param name="request">Token request parameters</param>
    /// <returns>Token response with access token</returns>
    /// <response code="200">Returns the access token</response>
    /// <response code="400">Invalid request parameters</response>
    /// <response code="401">Invalid client credentials</response>
    [HttpPost("token")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(TokenResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(object), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Token([FromBody] TokenRequest request)
    {
        try
        {
            var tokenResponse = await _tokenService.GenerateTokenAsync(request);
            return Ok(tokenResponse);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating token for client {ClientId}", request.ClientId);
            return BadRequest(new { error = "invalid_grant", error_description = ex.Message });
        }
    }

    /// <summary>
    /// Validate an access token
    /// </summary>
    /// <param name="tokenRequest">Token to validate</param>
    /// <returns>Token validation result</returns>
    /// <response code="200">Token validation result</response>
    /// <response code="400">Invalid token format</response>
    /// <response code="401">Unauthorized request</response>
    [HttpPost("validate")]
   // [Authorize]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> ValidateToken([FromBody] ValidateTokenRequest tokenRequest)
    {
        try
        {
            var isValid = await _tokenService.ValidateTokenAsync(tokenRequest.Token);
            return Ok(new { valid = isValid });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating token");
            return BadRequest(new { error = "invalid_token", error_description = ex.Message });
        }
    }

    /// <summary>
    /// Refresh an access token using refresh token
    /// </summary>
    /// <param name="refresh_token">Refresh token</param>
    /// <returns>New access token</returns>
    /// <response code="200">Returns the new access token</response>
    /// <response code="400">Invalid refresh token</response>
    [HttpPost("refresh")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(TokenResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RefreshToken([FromForm] string refresh_token)
    {
        try
        {
            var tokenResponse = await _tokenService.RefreshTokenAsync(refresh_token);
            return Ok(tokenResponse);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error refreshing token");
            return BadRequest(new { error = "invalid_grant", error_description = ex.Message });
        }
    }

    /// <summary>
    /// Revoke an access token
    /// </summary>
    /// <param name="token">Token to revoke</param>
    /// <returns>Revocation result</returns>
    /// <response code="200">Token revocation result</response>
    /// <response code="400">Invalid request</response>
    /// <response code="401">Unauthorized request</response>
    [HttpPost("revoke")]
    [Authorize]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> RevokeToken([FromForm] string token)
    {
        try
        {
            var revoked = await _tokenService.RevokeTokenAsync(token);
            return Ok(new { revoked });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error revoking token");
            return BadRequest(new { error = "invalid_request", error_description = ex.Message });
        }
    }
}

/// <summary>
/// Request model for token validation
/// </summary>
public class ValidateTokenRequest
{
    /// <summary>
    /// Token to validate
    /// </summary>
    public string Token { get; set; } = string.Empty;
}