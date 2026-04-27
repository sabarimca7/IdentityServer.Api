using MediatR;
using AutoMapper;
using IdentityServer.Application.DTOs;
using IdentityServer.Application.Interfaces;

namespace IdentityServer.Application.Commands;

public class DeleteUserCommand : IRequest<bool>
{
    public int Id { get; set; }
}

public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, bool>
{
    private readonly IUserService _userService;

    public DeleteUserCommandHandler(IUserService userService)
    {
        _userService = userService;
    }

    public async Task<bool> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        return await _userService.DeleteUserAsync(request.Id);
    }
}