using IdentityServer.Domain.Entities;

namespace IdentityServer.Application.Interfaces;

public interface IAuthorizationCodeService
{
    Task<AuthorizationCode?> GetByCodeAsync(string code);
    Task<bool> MarkAsUsedAsync(string code);
    Task<AuthorizationCode> CreateAsync(AuthorizationCode code);
}
