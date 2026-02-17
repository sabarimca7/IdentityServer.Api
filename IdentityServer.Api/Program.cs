using FluentValidation;
using IdentityServer.Api.Extensions;
using IdentityServer.Api.Middleware;
using IdentityServer.Application;
using IdentityServer.Application.Interfaces;
using IdentityServer.Application.Mappings;
using IdentityServer.Application.Services;
using IdentityServer.Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration));

// Add services to the container
builder.Services.AddControllers();

// Add Swagger Documentation
builder.Services.AddSwaggerDocumentation();

// Add Application Services
builder.Services.AddApplication();

// Add Infrastructure Services
builder.Services.AddInfrastructure(builder.Configuration);

// Add Security Services
builder.Services.AddScoped<IPasswordHashingService, PasswordHashingService>();
builder.Services.AddScoped<IClientSecretHashingService, ClientSecretHashingService>();

// Add AutoMapper
builder.Services.AddAutoMapper(typeof(MappingProfile));

// Add MediatR
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(MappingProfile).Assembly));

// Add FluentValidation
builder.Services.AddValidatorsFromAssemblyContaining<MappingProfile>();

// Add JWT Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]
                ?? "DefaultSecretKeyForJwtTokenGeneration123456789")),
            ClockSkew = TimeSpan.Zero
        };
    });

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigins",
        policy =>
        {
            policy.WithOrigins(
                    "http://localhost:3000",
                    "https://localhost:3001",
                    "http://localhost:5000")
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials();
        });
});

var app = builder.Build();


// 🔥 IMPORTANT: Must match IIS Application Alias
app.UsePathBase("/PGIdentityServer");


// Configure middleware pipeline
app.UseHttpsRedirection();

app.UseStaticFiles(); // Needed for Swagger UI static files

app.UseCors("AllowSpecificOrigins");

// Swagger
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("./v1/swagger.json", "IdentityServer API v1");
    c.RoutePrefix = "swagger";
});

// Custom Exception Middleware
app.UseMiddleware<ExceptionMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

app.UseRouting();
app.MapControllers();

// Redirect root to swagger
app.MapGet("/", () => Results.Redirect("swagger"))
   .ExcludeFromDescription();

app.Run();

public partial class Program { }
