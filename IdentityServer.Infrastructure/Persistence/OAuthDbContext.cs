using Microsoft.EntityFrameworkCore;
using IdentityServer.Infrastructure.Persistence.Models;

namespace IdentityServer.Infrastructure.Persistence;

public class OAuthDbContext : DbContext
{
    public OAuthDbContext(DbContextOptions<OAuthDbContext> options) : base(options)
    {
    }

    public DbSet<AppSettingModel> AppSettings { get; set; }
    public DbSet<ClientModel> Clients { get; set; }
    public DbSet<ExceptionLogModel> ExceptionLogs { get; set; }
    public DbSet<GrantTypeModel> GrantTypes { get; set; }
    public DbSet<RefreshTokenModel> RefreshTokens { get; set; }
    public DbSet<RequestResponseLogModel> RequestResponseLogs { get; set; }
    public DbSet<ScopeModel> Scopes { get; set; }
    public DbSet<UserModel> Users { get; set; }
    public DbSet<UserClientScopeModel> UserClientScopes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure Client entity
        modelBuilder.Entity<ClientModel>(entity =>
        {
            entity.HasKey(e => e.OAuthClientId);
            entity.Property(e => e.CreatedOn).HasDefaultValueSql("getdate()");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.AccessTokenValidity).HasDefaultValue(60);

            entity.HasOne(d => d.GrantType)
                .WithMany(p => p.Clients)
                .HasForeignKey(d => d.GrantTypeId)
                .HasConstraintName("FK_Client_GrantType");
        });

        // Configure User entity
        modelBuilder.Entity<UserModel>(entity =>
        {
            entity.HasKey(e => e.UserId);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.CreatedOn).HasDefaultValueSql("getdate()");
        });

        // Configure UserClientScope entity
        modelBuilder.Entity<UserClientScopeModel>(entity =>
        {
            entity.HasKey(e => e.UserClientScopeId);

            entity.HasOne(d => d.Client)
                .WithMany(p => p.UserClientScopes)
                .HasForeignKey(d => d.OAuthClientId)
                .HasConstraintName("FK_UserClientScope_Client");

            entity.HasOne(d => d.Scope)
                .WithMany(p => p.UserClientScopes)
                .HasForeignKey(d => d.ScopeId)
                .HasConstraintName("FK_UserClientScope_Scope");

            entity.HasOne(d => d.User)
                .WithMany(p => p.UserClientScopes)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_UserClientScope_User");
        });

        // Configure RefreshToken entity
        modelBuilder.Entity<RefreshTokenModel>(entity =>
        {
            entity.HasKey(e => e.RefreshTokenId);
        });
    }
}