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

// ─── Configure Serilog ────────────────────────────────────────────────────────
builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration));

// ─── Add Services ─────────────────────────────────────────────────────────────
builder.Services.AddControllers();

// Swagger
builder.Services.AddSwaggerDocumentation();

// Application & Infrastructure layers
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

// Security Services
builder.Services.AddScoped<IPasswordHashingService, PasswordHashingService>();
builder.Services.AddScoped<IClientSecretHashingService, ClientSecretHashingService>();


// AutoMapper
builder.Services.AddAutoMapper(typeof(MappingProfile));

// MediatR
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(MappingProfile).Assembly));

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
    });

// FluentValidation
builder.Services.AddValidatorsFromAssemblyContaining<MappingProfile>();

// JWT Authentication
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
                Encoding.UTF8.GetBytes(
                    builder.Configuration["Jwt:Key"]
                    ?? "DefaultSecretKeyForJwtTokenGeneration123456789")),
            ClockSkew = TimeSpan.Zero
        };
    });

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
              //.AllowCredentials();
    });
});

// ─── Build App ────────────────────────────────────────────────────────────────
var app = builder.Build();

// Must match IIS Application Alias — set before any middleware
app.UsePathBase("/PGIdentityServer");

// 1️⃣ Exception handler — catches everything, must be first
app.UseMiddleware<ExceptionMiddleware>();

// 2️⃣ HTTPS redirection
app.UseHttpsRedirection();

// 3️⃣ Static files (needed for Swagger UI assets)
app.UseStaticFiles();

// 4️⃣ Swagger (before routing/auth so it is always accessible)
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("./v1/swagger.json", "IdentityServer API v1");
    c.RoutePrefix = "swagger";
});

// 5️⃣ Routing — must come before Auth middleware
app.UseRouting();

// 6️⃣ CORS — must come after UseRouting and before UseAuthentication
app.UseCors("AllowAll");

// 7️⃣ Authentication & Authorization — order matters
app.UseAuthentication();
app.UseAuthorization();

// 8️⃣ Map endpoints
app.MapControllers();

// Redirect root → Swagger
app.MapGet("/", () => Results.Redirect("swagger"))
   .ExcludeFromDescription();
app.UseSwagger();
app.UseSwaggerUI();

app.Run();

public partial class Program { }