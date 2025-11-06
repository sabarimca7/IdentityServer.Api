using IdentityServer.Domain.Entities;

namespace IdentityServer.Application.Interfaces;

public interface IUserService
{
    Task<IEnumerable<User>> GetAllUsersAsync();
    Task<User?> GetUserByIdAsync(int id);
    Task<User?> GetUserByUsernameAsync(string username);
    Task<User> CreateUserAsync(User user);
    Task<User> UpdateUserAsync(User user);
    Task<bool> DeleteUserAsync(int id);
    Task<bool> ValidateUserCredentialsAsync(string username, string password);
    Task<bool> IncrementAccessFailedCountAsync(int userId);
    Task<bool> ResetAccessFailedCountAsync(int userId);
    Task<bool> LockUserAsync(int userId, TimeSpan lockoutDuration);
}