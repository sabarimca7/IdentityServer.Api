using MediatR;
using IdentityServer.Application.DTOs;

namespace IdentityServer.Application.Queries;

public class GetUsersQuery : IRequest<IEnumerable<UserDto>>
{
}