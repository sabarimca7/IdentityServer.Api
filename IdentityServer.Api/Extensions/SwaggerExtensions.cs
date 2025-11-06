using Microsoft.OpenApi.Models;
using System.Reflection;

namespace IdentityServer.Api.Extensions;

public static class SwaggerExtensions
{
    public static IServiceCollection AddSwaggerDocumentation(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "IdentityServer API",
                Version = "v1",
                Description = "OAuth 2.0 Identity Server API for token management and authentication",
                Contact = new OpenApiContact
                {
                    Name = "Identity Server",
                    Email = "support@identityserver.com"
                },
                License = new OpenApiLicense
                {
                    Name = "MIT License",
                    Url = new Uri("https://opensource.org/licenses/MIT")
                }
            });

            // Include XML comments
            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            if (File.Exists(xmlPath))
            {
                c.IncludeXmlComments(xmlPath);
            }

            // Add JWT authentication to Swagger
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer",
                BearerFormat = "JWT"
            });

            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });

            // OAuth2 flows for token endpoints
            c.AddSecurityDefinition("OAuth2", new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.OAuth2,
                Flows = new OpenApiOAuthFlows
                {
                    ClientCredentials = new OpenApiOAuthFlow
                    {
                        TokenUrl = new Uri("/api/token/token", UriKind.Relative),
                        Scopes = new Dictionary<string, string>
                        {
                            {"read", "Read access"},
                            {"write", "Write access"},
                            {"admin", "Admin access"},
                            {"profile", "Profile access"},
                            {"email", "Email access"}
                        }
                    },
                    Password = new OpenApiOAuthFlow
                    {
                        TokenUrl = new Uri("/api/token/token", UriKind.Relative),
                        Scopes = new Dictionary<string, string>
                        {
                            {"read", "Read access"},
                            {"write", "Write access"},
                            {"profile", "Profile access"},
                            {"email", "Email access"},
                            {"offline_access", "Offline access"}
                        }
                    }
                }
            });

            // Group endpoints by controller
            c.TagActionsBy(api => new[] { api.GroupName ?? api.ActionDescriptor.RouteValues["controller"] });
            c.DocInclusionPredicate((name, api) => true);
        });

        return services;
    }

    public static IApplicationBuilder UseSwaggerDocumentation(this IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment() || env.IsStaging())
        {
            app.UseSwagger(c =>
            {
                c.RouteTemplate = "swagger/{documentName}/swagger.json";
            });

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "IdentityServer API v1");
                c.RoutePrefix = "swagger";
                c.DisplayRequestDuration();
                c.EnableValidator();
                c.EnableDeepLinking();
                c.ShowExtensions();
                c.EnableFilter();
                c.MaxDisplayedTags(10);

                // OAuth2 configuration
                c.OAuthClientId("swagger-ui");
                c.OAuthAppName("IdentityServer Swagger UI");
                c.OAuthUsePkce();
            });
        }

        return app;
    }
}