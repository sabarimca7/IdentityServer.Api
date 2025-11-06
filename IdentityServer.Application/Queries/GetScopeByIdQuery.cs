using MediatR;
using IdentityServer.Application.DTOs;

namespace IdentityServer.Application.Queries;

public class GetScopeByIdQuery : IRequest<ScopeDto?>
{
    public int Id { get; set; }
}