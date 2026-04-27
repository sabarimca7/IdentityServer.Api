using AutoMapper;
using Microsoft.EntityFrameworkCore;
using IdentityServer.Application.Interfaces;
using IdentityServer.Domain.Entities;
using IdentityServer.Infrastructure.Persistence;
using IdentityServer.Infrastructure.Persistence.Models;
using System.Web;

namespace IdentityServer.Infrastructure.Repositories;

public class RefreshTokenRepository : IRefreshTokenService
{
    private readonly OAuthDbContext _context;
    private readonly IMapper _mapper;

    public RefreshTokenRepository(OAuthDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<RefreshToken> CreateRefreshTokenAsync(RefreshToken refreshToken)
    {
        var refreshTokenModel = _mapper.Map<RefreshTokenModel>(refreshToken);
        _context.RefreshTokens.Add(refreshTokenModel);
        await _context.SaveChangesAsync();
        return _mapper.Map<RefreshToken>(refreshTokenModel);
    }

    public async Task<RefreshToken?> GetRefreshTokenByTokenAsync(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
            return null;

        // Normalize token
        var normalized = token.Trim();

        // Try URL decode (some clients/clients put tokens into URLs)
        try
        {
            normalized = HttpUtility.UrlDecode(normalized) ?? normalized;
        }
        catch
        {
            // ignore decoding errors
        }

        // If token looks like a JWT (has dots) it's likely an access token, not a refresh token.
        if (normalized.Contains('.'))
        {
            return null;
        }

        // Ensure base64 padding if token looks like base64
        if (IsBase64String(normalized) && normalized.Length % 4 != 0)
        {
            normalized = PadBase64(normalized);
        }

        // Try exact match first
        var refreshToken = await _context.RefreshTokens
            .FirstOrDefaultAsync(rt => rt.Token == normalized);
        if (refreshToken != null) return _mapper.Map<RefreshToken>(refreshToken);

        // Try case-insensitive match
        refreshToken = await _context.RefreshTokens
            .FirstOrDefaultAsync(rt => rt.Token != null && rt.Token.ToLower() == normalized.ToLower());
        if (refreshToken != null) return _mapper.Map<RefreshToken>(refreshToken);

        // As a last resort try tokens that contain the provided value (helps with truncated/prefixed tokens)
        refreshToken = await _context.RefreshTokens
            .FirstOrDefaultAsync(rt => rt.Token != null && EF.Functions.Like(rt.Token, $"%{normalized}%"));

        return refreshToken != null ? _mapper.Map<RefreshToken>(refreshToken) : null;
    }

    public async Task<bool> RevokeRefreshTokenAsync(string token)
    {
        var refreshToken = await _context.RefreshTokens
            .FirstOrDefaultAsync(rt => rt.Token == token);
        
        if (refreshToken != null)
        {
            refreshToken.IsRevoked = true;
            _context.RefreshTokens.Update(refreshToken);
            await _context.SaveChangesAsync();
            return true;
        }
        return false;
    }

    public async Task<bool> RevokeRefreshTokenByIdAsync(int refreshTokenId)
    {
        var refreshToken = await _context.RefreshTokens.FindAsync(refreshTokenId);
        if (refreshToken != null)
        {
            refreshToken.IsRevoked = true;
            _context.RefreshTokens.Update(refreshToken);
            await _context.SaveChangesAsync();
            return true;
        }
        return false;
    }

    public async Task<bool> DeleteExpiredTokensAsync()
    {
        var expiredTokens = await _context.RefreshTokens
            .Where(rt => rt.ExpiresOn.HasValue && rt.ExpiresOn < DateTime.UtcNow)
            .ToListAsync();

        if (expiredTokens.Any())
        {
            _context.RefreshTokens.RemoveRange(expiredTokens);
            await _context.SaveChangesAsync();
            return true;
        }
        return false;
    }

    private static bool IsBase64String(string s)
    {
        Span<byte> buffer = new byte[s.Length];
        return Convert.TryFromBase64String(s, buffer, out _);
    }

    private static string PadBase64(string s)
    {
        var mod = s.Length % 4;
        if (mod == 0) return s;
        return s + new string('=', 4 - mod);
    }
}
