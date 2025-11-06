using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using FluentAssertions;
using IdentityServer.Api.Controllers;
using IdentityServer.Application.DTOs;
using IdentityServer.Application.Queries;
using IdentityServer.Application.Commands;
using IdentityServer.Common.Models;

namespace IdentityServer.Tests.Unit.Controllers;

public class ClientControllerTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly Mock<ILogger<ClientController>> _loggerMock;
    private readonly ClientController _controller;

    public ClientControllerTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _loggerMock = new Mock<ILogger<ClientController>>();
        _controller = new ClientController(_mediatorMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task GetClients_ValidRequest_ReturnsOkWithClients()
    {
        // Arrange
        var clients = new List<ClientDto>
        {
            new ClientDto { OAuthClientId = 1, ClientId = "client1", Name = "Test Client 1" },
            new ClientDto { OAuthClientId = 2, ClientId = "client2", Name = "Test Client 2" }
        };

        _mediatorMock.Setup(x => x.Send(It.IsAny<GetClientsQuery>(), default))
            .ReturnsAsync(clients);

        // Act
        var result = await _controller.GetClients();

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var response = okResult.Value.Should().BeOfType<ApiResponse<IEnumerable<ClientDto>>>().Subject;
        response.Success.Should().BeTrue();
        response.Data.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetClient_ExistingId_ReturnsOkWithClient()
    {
        // Arrange
        var clientId = 1;
        var client = new ClientDto { OAuthClientId = clientId, ClientId = "client1", Name = "Test Client" };

        _mediatorMock.Setup(x => x.Send(It.Is<GetClientByIdQuery>(q => q.Id == clientId), default))
            .ReturnsAsync(client);

        // Act
        var result = await _controller.GetClient(clientId);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var response = okResult.Value.Should().BeOfType<ApiResponse<ClientDto>>().Subject;
        response.Success.Should().BeTrue();
        response.Data.Should().NotBeNull();
        response.Data!.OAuthClientId.Should().Be(clientId);
    }

    [Fact]
    public async Task GetClient_NonExistingId_ReturnsNotFound()
    {
        // Arrange
        var clientId = 999;

        _mediatorMock.Setup(x => x.Send(It.Is<GetClientByIdQuery>(q => q.Id == clientId), default))
            .ReturnsAsync((ClientDto?)null);

        // Act
        var result = await _controller.GetClient(clientId);

        // Assert
        var notFoundResult = result.Result.Should().BeOfType<NotFoundObjectResult>().Subject;
        var response = notFoundResult.Value.Should().BeOfType<ApiResponse<ClientDto>>().Subject;
        response.Success.Should().BeFalse();
        response.Message.Should().Be("Client not found");
    }

    [Fact]
    public async Task CreateClient_ValidClient_ReturnsCreatedResult()
    {
        // Arrange
        var createClientDto = new CreateClientDto
        {
            ClientId = "new_client",
            ClientSecret = "new_secret",
            Name = "New Client"
        };

        var createdClient = new ClientDto
        {
            OAuthClientId = 1,
            ClientId = createClientDto.ClientId,
            Name = createClientDto.Name
        };

        _mediatorMock.Setup(x => x.Send(It.IsAny<CreateClientCommand>(), default))
            .ReturnsAsync(createdClient);

        // Act
        var result = await _controller.CreateClient(createClientDto);

        // Assert
        var createdResult = result.Result.Should().BeOfType<CreatedAtActionResult>().Subject;
        var response = createdResult.Value.Should().BeOfType<ApiResponse<ClientDto>>().Subject;
        response.Success.Should().BeTrue();
        response.Data.Should().NotBeNull();
        response.Data!.ClientId.Should().Be(createClientDto.ClientId);
    }
}