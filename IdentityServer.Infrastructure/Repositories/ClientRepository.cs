using AutoMapper;
using Microsoft.EntityFrameworkCore;
using IdentityServer.Application.Interfaces;
using IdentityServer.Domain.Entities;
using IdentityServer.Infrastructure.Persistence;
using IdentityServer.Infrastructure.Persistence.Models;

namespace IdentityServer.Infrastructure.Repositories;

public class ClientRepository : IClientService
{
    private readonly OAuthDbContext _context;
    private readonly IMapper _mapper;

    public ClientRepository(OAuthDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<IEnumerable<Client>> GetAllClientsAsync()
    {
        var clients = await _context.Clients
            .Include(c => c.GrantType)
            .ToListAsync();

        return _mapper.Map<IEnumerable<Client>>(clients);
    }

    public async Task<Client?> GetClientByIdAsync(int id)
    {
        var client = await _context.Clients
            .Include(c => c.GrantType)
            .FirstOrDefaultAsync(c => c.OAuthClientId == id);

        return client != null ? _mapper.Map<Client>(client) : null;
    }

    public async Task<Client?> GetClientByClientIdAsync(string clientId)
    {
        var client = await _context.Clients
            .Include(c => c.GrantType)
            .FirstOrDefaultAsync(c => c.ClientId == clientId);

        return client != null ? _mapper.Map<Client>(client) : null;
    }

    public async Task<Client> CreateClientAsync(Client client)
    {
        var clientModel = _mapper.Map<ClientModel>(client);
        _context.Clients.Add(clientModel);
        await _context.SaveChangesAsync();

        return _mapper.Map<Client>(clientModel);
    }

    public async Task<Client> UpdateClientAsync(Client client)
    {
        var clientModel = _mapper.Map<ClientModel>(client);
        _context.Clients.Update(clientModel);
        await _context.SaveChangesAsync();

        return _mapper.Map<Client>(clientModel);
    }

    public async Task<bool> DeleteClientAsync(int id)
    {
        var client = await _context.Clients.FindAsync(id);
        if (client != null)
        {
            _context.Clients.Remove(client);
            await _context.SaveChangesAsync();
            return true;
        }
        return false;
    }

    public async Task<bool> ValidateClientCredentialsAsync(string clientId, string clientSecret)
    {
        var client = await _context.Clients
            .FirstOrDefaultAsync(c => c.ClientId == clientId && c.ClientSecret == clientSecret && c.IsActive);

        return client != null;
    }
}