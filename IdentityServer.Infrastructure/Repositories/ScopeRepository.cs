using AutoMapper;
using Microsoft.EntityFrameworkCore;
using IdentityServer.Application.Interfaces;
using IdentityServer.Domain.Entities;
using IdentityServer.Infrastructure.Persistence;
using IdentityServer.Infrastructure.Persistence.Models;

namespace IdentityServer.Infrastructure.Repositories;

public class ScopeRepository : IScopeService
{
    private readonly OAuthDbContext _context;
    private readonly IMapper _mapper;

    public ScopeRepository(OAuthDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<IEnumerable<Scope>> GetAllScopesAsync()
    {
        var scopes = await _context.Scopes.ToListAsync();
        return _mapper.Map<IEnumerable<Scope>>(scopes);
    }

    public async Task<Scope?> GetScopeByIdAsync(int id)
    {
        var scope = await _context.Scopes.FindAsync(id);
        return scope != null ? _mapper.Map<Scope>(scope) : null;
    }

    public async Task<Scope> CreateScopeAsync(Scope scope)
    {
        var scopeModel = _mapper.Map<ScopeModel>(scope);
        _context.Scopes.Add(scopeModel);
        await _context.SaveChangesAsync();
        return _mapper.Map<Scope>(scopeModel);
    }

    public async Task<Scope> UpdateScopeAsync(Scope scope)
    {
        var scopeModel = _mapper.Map<ScopeModel>(scope);
        _context.Scopes.Update(scopeModel);
        await _context.SaveChangesAsync();
        return _mapper.Map<Scope>(scopeModel);
    }

    public async Task<bool> DeleteScopeAsync(int id)
    {
        var scope = await _context.Scopes.FindAsync(id);
        if (scope != null)
        {
            _context.Scopes.Remove(scope);
            await _context.SaveChangesAsync();
            return true;
        }
        return false;
    }
}