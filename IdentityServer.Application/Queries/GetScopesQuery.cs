using MediatR;
using IdentityServer.Application.DTOs;

namespace IdentityServer.Application.Queries;

public class GetScopesQuery : IRequest<IEnumerable<ScopeDto>>
{
}