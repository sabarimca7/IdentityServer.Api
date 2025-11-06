using Microsoft.Extensions.DependencyInjection;
using IdentityServer.Application.Interfaces;
using IdentityServer.Application.Services;

namespace IdentityServer.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<ITokenService, TokenService>();

        return services;
    }
}