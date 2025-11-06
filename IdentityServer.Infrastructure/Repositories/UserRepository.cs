using AutoMapper;
using Microsoft.EntityFrameworkCore;
using IdentityServer.Application.Interfaces;
using IdentityServer.Domain.Entities;
using IdentityServer.Infrastructure.Persistence;
using IdentityServer.Infrastructure.Persistence.Models;

namespace IdentityServer.Infrastructure.Repositories;

public class UserRepository : IUserService
{
    private readonly OAuthDbContext _context;
    private readonly IMapper _mapper;

    public UserRepository(OAuthDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<IEnumerable<User>> GetAllUsersAsync()
    {
        var users = await _context.Users.ToListAsync();
        return _mapper.Map<IEnumerable<User>>(users);
    }

    public async Task<User?> GetUserByIdAsync(int id)
    {
        var user = await _context.Users.FindAsync(id);
        return user != null ? _mapper.Map<User>(user) : null;
    }

    public async Task<User?> GetUserByUsernameAsync(string username)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
        return user != null ? _mapper.Map<User>(user) : null;
    }

    public async Task<User> CreateUserAsync(User user)
    {
        var userModel = _mapper.Map<UserModel>(user);
        userModel.CreatedOn = DateTime.UtcNow;
        _context.Users.Add(userModel);
        await _context.SaveChangesAsync();
        return _mapper.Map<User>(userModel);
    }

    public async Task<User> UpdateUserAsync(User user)
    {
        var userModel = _mapper.Map<UserModel>(user);
        _context.Users.Update(userModel);
        await _context.SaveChangesAsync();
        return _mapper.Map<User>(userModel);
    }

    public async Task<bool> DeleteUserAsync(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user != null)
        {
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return true;
        }
        return false;
    }

    public async Task<bool> ValidateUserCredentialsAsync(string username, string password)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Username == username && u.Password == password && u.IsActive);

        return user != null && (user.LockoutEnd == null || user.LockoutEnd <= DateTime.UtcNow);
    }

    public async Task<bool> IncrementAccessFailedCountAsync(int userId)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user != null)
        {
            user.AccessFailedCount = (user.AccessFailedCount ?? 0) + 1;
            await _context.SaveChangesAsync();
            return true;
        }
        return false;
    }
    public async Task<bool> ResetAccessFailedCountAsync(int userId)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user != null)
        {
            user.AccessFailedCount = 0;
            user.LockoutEnd = null;
            await _context.SaveChangesAsync();
            return true;
        }
        return false;
    }

    public async Task<bool> LockUserAsync(int userId, TimeSpan lockoutDuration)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user != null)
        {
            user.LockoutEnd = DateTime.UtcNow.Add(lockoutDuration);
            await _context.SaveChangesAsync();
            return true;
        }
        return false;
    }
}