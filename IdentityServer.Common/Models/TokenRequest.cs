namespace IdentityServer.Common.Models;

public class TokenRequest
{
    public string GrantType { get; set; } = string.Empty;
    public string ClientId { get; set; } = string.Empty;
    public string ClientSecret { get; set; } = string.Empty;
    public string? Username { get; set; }
    public string? Password { get; set; }
    public string? Code { get; set; }
    public string? RedirectUri { get; set; }
    public string? RefreshToken { get; set; }
    public string? Scope { get; set; }
}