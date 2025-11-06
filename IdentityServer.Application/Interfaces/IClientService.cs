using IdentityServer.Domain.Entities;

namespace IdentityServer.Application.Interfaces;

public interface IClientService
{
    Task<IEnumerable<Client>> GetAllClientsAsync();
    Task<Client?> GetClientByIdAsync(int id);
    Task<Client?> GetClientByClientIdAsync(string clientId);
    Task<Client> CreateClientAsync(Client client);
    Task<Client> UpdateClientAsync(Client client);
    Task<bool> DeleteClientAsync(int id);
    Task<bool> ValidateClientCredentialsAsync(string clientId, string clientSecret);
}