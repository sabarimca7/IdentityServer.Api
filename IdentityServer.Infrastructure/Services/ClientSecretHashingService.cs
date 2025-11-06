using System.Security.Cryptography;
using System.Text;

namespace IdentityServer.Application.Services;

public interface IClientSecretHashingService
{
    string HashClientSecret(string secret);
    bool VerifyClientSecret(string secret, string hash);
}

public class ClientSecretHashingService : IClientSecretHashingService
{
    public string HashClientSecret(string secret)
    {
        using var sha256 = SHA256.Create();
        var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(secret));
        return Convert.ToBase64String(hashedBytes);
    }

    public bool VerifyClientSecret(string secret, string hash)
    {
        var hashedSecret = HashClientSecret(secret);
        return hashedSecret == hash;
    }
}