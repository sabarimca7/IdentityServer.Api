using AutoMapper;
using IdentityServer.Application.DTOs;
using IdentityServer.Application.Interfaces;
using MediatR;

namespace IdentityServer.Application.Queries;

public class GetUsersQuery : IRequest<IEnumerable<UserDto>>
{
}
public class GetUsersQueryHandler : IRequestHandler<GetUsersQuery, IEnumerable<UserDto>>
{
    private readonly IUserService _userService;
    private readonly IMapper _mapper;

    public GetUsersQueryHandler(IUserService userService, IMapper mapper)
    {
        _userService = userService;
        _mapper = mapper;
    }

    public async Task<IEnumerable<UserDto>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
    {
        var users = await _userService.GetAllUsersAsync();
        return _mapper.Map<IEnumerable<UserDto>>(users);
    }
}