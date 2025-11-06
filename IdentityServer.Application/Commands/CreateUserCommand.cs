using AutoMapper;
using IdentityServer.Application.DTOs;
using IdentityServer.Application.Interfaces;
using IdentityServer.Common.Models;
using IdentityServer.Domain.Entities;
using MediatR;

namespace IdentityServer.Application.Commands;

public class CreateUserCommand : IRequest<ApiResponse<UserDto>>
{
    public CreateUserDto UserDto { get; set; } = null!;
}

public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, ApiResponse<UserDto>>
{
    private readonly IUserService _userService;
    private readonly IMapper _mapper;

    public CreateUserCommandHandler(IUserService userService, IMapper mapper)
    {
        _userService = userService;
        _mapper = mapper;
    }

    public async Task<ApiResponse<UserDto>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var user = _mapper.Map<User>(request.UserDto);
        user.CreatedOn = DateTime.UtcNow;

        var existingUser = await _userService.GetUserByUsernameAsync(user.Username);
        if (existingUser != null)
        {
            return ApiResponse<UserDto>.ErrorResponse("User already exists");
        }
        // Need Hash password
      // var hashedPassword 
        var users = new User
        {
            Username = user.Username,
            Password = user.Password,
            Firstname = user.Firstname,
            Lastname = user.Lastname,
            EmailAddress = user.EmailAddress,
            IsActive = user.IsActive,
            CreatedOn = DateTime.UtcNow,
            AccessFailedCount = 0
        };
        var createduser = await _userService.CreateUserAsync(users);
       var mappedUser =  _mapper.Map<UserDto>(createduser);
        return ApiResponse<UserDto>.SuccessResponse(mappedUser, "User created successfully");
    }
}