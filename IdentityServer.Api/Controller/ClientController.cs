using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using IdentityServer.Application.Commands;
using IdentityServer.Application.DTOs;
using IdentityServer.Application.Queries;
using IdentityServer.Common.Models;

namespace IdentityServer.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ClientController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<ClientController> _logger;

    public ClientController(IMediator mediator, ILogger<ClientController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<IEnumerable<ClientDto>>>> GetClients()
    {
        try
        {
            var clients = await _mediator.Send(new GetClientsQuery());
            return Ok(ApiResponse<IEnumerable<ClientDto>>.SuccessResponse(clients));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving clients");
            return StatusCode(500, ApiResponse<IEnumerable<ClientDto>>.ErrorResponse("Internal server error"));
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<ClientDto>>> GetClient(int id)
    {
        try
        {
            var client = await _mediator.Send(new GetClientByIdQuery { Id = id });
            if (client == null)
                return NotFound(ApiResponse<ClientDto>.ErrorResponse("Client not found"));

            return Ok(ApiResponse<ClientDto>.SuccessResponse(client));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving client {Id}", id);
            return StatusCode(500, ApiResponse<ClientDto>.ErrorResponse("Internal server error"));
        }
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<ClientDto>>> CreateClient([FromBody] CreateClientDto clientDto)
    {
        try
        {
            var client = await _mediator.Send(new CreateClientCommand { ClientDto = clientDto });
            return CreatedAtAction(nameof(GetClient), new { id = client.OAuthClientId },
                ApiResponse<ClientDto>.SuccessResponse(client, "Client created successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating client");
            return StatusCode(500, ApiResponse<ClientDto>.ErrorResponse("Internal server error"));
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<ClientDto>>> UpdateClient(int id, [FromBody] UpdateClientDto clientDto)
    {
        try
        {
            if (id != clientDto.OAuthClientId)
                return BadRequest(ApiResponse<ClientDto>.ErrorResponse("ID mismatch"));

            var client = await _mediator.Send(new UpdateClientCommand { ClientDto = clientDto });
            return Ok(ApiResponse<ClientDto>.SuccessResponse(client, "Client updated successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating client {Id}", id);
            return StatusCode(500, ApiResponse<ClientDto>.ErrorResponse("Internal server error"));
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse<bool>>> DeleteClient(int id)
    {
        try
        {
            var result = await _mediator.Send(new DeleteClientCommand { Id = id });
            if (!result)
                return NotFound(ApiResponse<bool>.ErrorResponse("Client not found"));

            return Ok(ApiResponse<bool>.SuccessResponse(true, "Client deleted successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting client {Id}", id);
            return StatusCode(500, ApiResponse<bool>.ErrorResponse("Internal server error"));
        }
    }
}