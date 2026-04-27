using AutoMapper;
using Microsoft.EntityFrameworkCore;
using IdentityServer.Application.Interfaces;
using IdentityServer.Domain.Entities;
using IdentityServer.Infrastructure.Persistence;
using IdentityServer.Infrastructure.Persistence.Models;

namespace IdentityServer.Infrastructure.Repositories;

public class AuthorizationCodeRepository : IAuthorizationCodeService
{
    private readonly OAuthDbContext _context;
    private readonly IMapper _mapper;

    public AuthorizationCodeRepository(OAuthDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<AuthorizationCode?> GetByCodeAsync(string code)
    {
        try
        {
            var model = await _context.Set<AuthorizationCodeModel>()
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.Code == code);
            return model != null ? _mapper.Map<AuthorizationCode>(model) : null;
        }
        catch (Exception ex)
        {
            // If the AuthorizationCode table does not exist, return null instead of throwing
            if (ex.Message != null && ex.Message.Contains("Invalid object name", StringComparison.OrdinalIgnoreCase))
                return null;

            throw;
        }
    }

    public async Task<bool> MarkAsUsedAsync(string code)
    {
        try
        {
            var model = await _context.Set<AuthorizationCodeModel>()
                .FirstOrDefaultAsync(a => a.Code == code);
            if (model == null) return false;
            model.IsUsed = true;
            _context.Update(model);
            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            // If table missing, treat as no-op and return false
            if (ex.Message != null && ex.Message.Contains("Invalid object name", StringComparison.OrdinalIgnoreCase))
                return false;

            throw;
        }
    }

    public async Task<AuthorizationCode> CreateAsync(AuthorizationCode code)
    {
        try
        {
            var model = _mapper.Map<AuthorizationCodeModel>(code);
            _context.Set<AuthorizationCodeModel>().Add(model);
            await _context.SaveChangesAsync();
            return _mapper.Map<AuthorizationCode>(model);
        }
        catch (Exception ex)
        {
            // If table missing, return the input code object without persisting
            if (ex.Message != null && ex.Message.Contains("Invalid object name", StringComparison.OrdinalIgnoreCase))
                return code;

            throw;
        }
    }
}
