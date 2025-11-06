using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using IdentityServer.Application.DTOs;
using IdentityServer.Application.Queries;
using IdentityServer.Common.Models;

namespace IdentityServer.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ScopeController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<ScopeController> _logger;

    public ScopeController(IMediator mediator, ILogger<ScopeController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<IEnumerable<ScopeDto>>>> GetScopes()
    {
        try
        {
            var scopes = await _mediator.Send(new GetScopesQuery());
            return Ok(ApiResponse<IEnumerable<ScopeDto>>.SuccessResponse(scopes));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving scopes");
            return StatusCode(500, ApiResponse<IEnumerable<ScopeDto>>.ErrorResponse("Internal server error"));
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<ScopeDto>>> GetScope(int id)
    {
        try
        {
            var scope = await _mediator.Send(new GetScopeByIdQuery { Id = id });
            if (scope == null)
                return NotFound(ApiResponse<ScopeDto>.ErrorResponse("Scope not found"));

            return Ok(ApiResponse<ScopeDto>.SuccessResponse(scope));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving scope {Id}", id);
            return StatusCode(500, ApiResponse<ScopeDto>.ErrorResponse("Internal server error"));
        }
    }
}