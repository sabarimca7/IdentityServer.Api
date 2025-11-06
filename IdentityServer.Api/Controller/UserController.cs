using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using IdentityServer.Application.DTOs;
using IdentityServer.Application.Queries;
using IdentityServer.Application.Commands;
using IdentityServer.Common.Models;

namespace IdentityServer.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UserController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<UserController> _logger;

    public UserController(IMediator mediator, ILogger<UserController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<IEnumerable<UserDto>>>> GetUsers()
    {
        try
        {
            var users = await _mediator.Send(new GetUsersQuery());
            return Ok(ApiResponse<IEnumerable<UserDto>>.SuccessResponse(users));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving users");
            return StatusCode(500, ApiResponse<IEnumerable<UserDto>>.ErrorResponse("Internal server error"));
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<UserDto>>> GetUser(int id)
    {
        try
        {
            var user = await _mediator.Send(new GetUserByIdQuery { Id = id });
            if (user == null)
                return NotFound(ApiResponse<UserDto>.ErrorResponse("User not found"));

            return Ok(ApiResponse<UserDto>.SuccessResponse(user));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving user {Id}", id);
            return StatusCode(500, ApiResponse<UserDto>.ErrorResponse("Internal server error"));
        }
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<UserDto>>> CreateUser([FromBody] CreateUserDto userDto)
    {
        try
        {
            var user = await _mediator.Send(new CreateUserCommand { UserDto = userDto });
            if (user == null)
                return NotFound(ApiResponse<UserDto>.ErrorResponse("User not found"));

            return Ok(user);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating user");
            return StatusCode(500, ApiResponse<UserDto>.ErrorResponse("Internal server error"));
        }
    }
}