using IdentityServer.Application.Interfaces;
using IdentityServer.Infrastructure.Mappings;
using IdentityServer.Infrastructure.Persistence;
using IdentityServer.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<OAuthDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        services.AddAutoMapper(typeof(InfrastructureMappingProfile));

        services.AddScoped<IClientService, ClientRepository>();
        services.AddScoped<IUserService, UserRepository>();
        return services;
    }
}
