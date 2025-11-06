using MediatR;
using IdentityServer.Application.DTOs;

namespace IdentityServer.Application.Queries;

public class GetUserByIdQuery : IRequest<UserDto?>
{
    public int Id { get; set; }
}