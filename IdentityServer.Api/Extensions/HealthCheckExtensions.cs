using Microsoft.Extensions.Diagnostics.HealthChecks;
using IdentityServer.Infrastructure.Persistence;

namespace IdentityServer.Api.Extensions;

public static class HealthCheckExtensions
{
    public static IServiceCollection AddCustomHealthChecks(this IServiceCollection services, IConfiguration configuration)
    {
        //services.AddHealthChecks()
        //    .AddDbContextCheck<OAuthDbContext>("database")
        //    .AddCheck("self", () => HealthCheckResult.Healthy(), new[] { "self" });

        return services;
    }
}