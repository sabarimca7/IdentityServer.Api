using IdentityServer.Domain.Entities;

namespace IdentityServer.Application.Interfaces;

public interface IScopeService
{
    Task<IEnumerable<Scope>> GetAllScopesAsync();
    Task<Scope?> GetScopeByIdAsync(int id);
    Task<Scope> CreateScopeAsync(Scope scope);
    Task<Scope> UpdateScopeAsync(Scope scope);
    Task<bool> DeleteScopeAsync(int id);
}