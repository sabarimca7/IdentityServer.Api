using AutoMapper;
using IdentityServer.Application.DTOs;
using IdentityServer.Application.Interfaces;
using MediatR;

namespace IdentityServer.Application.Queries;

public class GetUserByIdQuery : IRequest<UserDto?>
{
    public int Id { get; set; }
}

public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, UserDto?>
{
    private readonly IUserService _userService;
    private readonly IMapper _mapper;

    public GetUserByIdQueryHandler(IUserService userService, IMapper mapper)
    {
        _userService = userService;
        _mapper = mapper;
    }

    public async Task<UserDto?> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        var user = await _userService.GetUserByIdAsync(request.Id);
        return user != null ? _mapper.Map<UserDto>(user) : null;
    }
}