namespace IdentityServer.Application.DTOs;

public class ClientDto
{
    public int OAuthClientId { get; set; }
    public string ClientId { get; set; } = string.Empty;
    public string ClientSecret { get; set; } = string.Empty;
    public DateTime CreatedOn { get; set; }
    public bool IsActive { get; set; }
    public string? Name { get; set; }
    public int? GrantTypeId { get; set; }
    public int? ApplicationId { get; set; }
    public int? AccessTokenValidity { get; set; }
    public string? GrantTypeName { get; set; }
}