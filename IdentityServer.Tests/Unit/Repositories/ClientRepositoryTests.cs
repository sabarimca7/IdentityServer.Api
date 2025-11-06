using Microsoft.EntityFrameworkCore;
using AutoMapper;
using Xunit;
using FluentAssertions;
using IdentityServer.Infrastructure.Persistence;
using IdentityServer.Infrastructure.Persistence.Models;
using IdentityServer.Infrastructure.Repositories;
using IdentityServer.Infrastructure.Mappings;
using IdentityServer.Domain.Entities;

namespace IdentityServer.Tests.Unit.Repositories;

public class ClientRepositoryTests : IDisposable
{
    private readonly OAuthDbContext _context;
    private readonly IMapper _mapper;
    private readonly ClientRepository _repository;

    public ClientRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<OAuthDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new OAuthDbContext(options);

        var config = new MapperConfiguration(cfg => cfg.AddProfile<InfrastructureMappingProfile>());
        _mapper = config.CreateMapper();

        _repository = new ClientRepository(_context, _mapper);

        SeedTestData();
    }

    private void SeedTestData()
    {
        var grantType = new GrantTypeModel
        {
            GrantTypeId = 1,
            Name = "client_credentials",
            ShortDescription = "Client Credentials Grant"
        };

        var client = new ClientModel
        {
            OAuthClientId = 1,
            ClientId = "test_client",
            ClientSecret = "test_secret",
            Name = "Test Client",
            IsActive = true,
            CreatedOn = DateTime.UtcNow,
            GrantTypeId = 1,
            AccessTokenValidity = 3600
        };

        _context.GrantTypes.Add(grantType);
        _context.Clients.Add(client);
        _context.SaveChanges();
    }

    [Fact]
    public async Task GetAllClientsAsync_ReturnsAllClients()
    {
        // Act
        var result = await _repository.GetAllClientsAsync();

        // Assert
        result.Should().NotBeEmpty();
        result.Should().HaveCount(1);
        result.First().ClientId.Should().Be("test_client");
    }

    [Fact]
    public async Task GetClientByIdAsync_ExistingId_ReturnsClient()
    {
        // Act
        var result = await _repository.GetClientByIdAsync(1);

        // Assert
        result.Should().NotBeNull();
        result!.ClientId.Should().Be("test_client");
        result.Name.Should().Be("Test Client");
    }

    [Fact]
    public async Task GetClientByIdAsync_NonExistingId_ReturnsNull()
    {
        // Act
        var result = await _repository.GetClientByIdAsync(999);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetClientByClientIdAsync_ExistingClientId_ReturnsClient()
    {
        // Act
        var result = await _repository.GetClientByClientIdAsync("test_client");

        // Assert
        result.Should().NotBeNull();
        result!.ClientId.Should().Be("test_client");
    }

    [Fact]
    public async Task CreateClientAsync_ValidClient_ReturnsCreatedClient()
    {
        // Arrange
        var newClient = new Client
        {
            ClientId = "new_client",
            ClientSecret = "new_secret",
            Name = "New Client",
            IsActive = true,
            CreatedOn = DateTime.UtcNow,
            AccessTokenValidity = 1800
        };

        // Act
        var result = await _repository.CreateClientAsync(newClient);

        // Assert
        result.Should().NotBeNull();
        result.ClientId.Should().Be("new_client");
        result.OAuthClientId.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task ValidateClientCredentialsAsync_ValidCredentials_ReturnsTrue()
    {
        // Act
        var result = await _repository.ValidateClientCredentialsAsync("test_client", "test_secret");

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task ValidateClientCredentialsAsync_InvalidCredentials_ReturnsFalse()
    {
        // Act
        var result = await _repository.ValidateClientCredentialsAsync("invalid_client", "invalid_secret");

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task UpdateClientAsync_ExistingClient_ReturnsUpdatedClient()
    {
        // Arrange
        var client = await _repository.GetClientByIdAsync(1);
        client!.Name = "Updated Client Name";
        client.AccessTokenValidity = 7200;

        // Act
        var result = await _repository.UpdateClientAsync(client);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be("Updated Client Name");
        result.AccessTokenValidity.Should().Be(7200);
    }

    [Fact]
    public async Task DeleteClientAsync_ExistingClient_ReturnsTrue()
    {
        // Act
        var result = await _repository.DeleteClientAsync(1);

        // Assert
        result.Should().BeTrue();

        // Verify deletion
        var deletedClient = await _repository.GetClientByIdAsync(1);
        deletedClient.Should().BeNull();
    }

    [Fact]
    public async Task DeleteClientAsync_NonExistingClient_ReturnsFalse()
    {
        // Act
        var result = await _repository.DeleteClientAsync(999);

        // Assert
        result.Should().BeFalse();
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}