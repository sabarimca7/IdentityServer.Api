using MediatR;
using AutoMapper;
using IdentityServer.Application.DTOs;
using IdentityServer.Application.Interfaces;
using IdentityServer.Domain.Entities;

namespace IdentityServer.Application.Commands;

public class UpdateUserCommand : IRequest<UserDto>
{
    public UpdateUserDto UserDto { get; set; } = null!;
}

public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, UserDto>
{
    private readonly IUserService _userService;
    private readonly IMapper _mapper;

    public UpdateUserCommandHandler(IUserService userService, IMapper mapper)
    {
        _userService = userService;
        _mapper = mapper;
    }

    public async Task<UserDto> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var user = _mapper.Map<User>(request.UserDto);
        var updatedUser = await _userService.UpdateUserAsync(user);
        return _mapper.Map<UserDto>(updatedUser);
    }
}