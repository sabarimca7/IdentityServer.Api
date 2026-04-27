using AutoMapper;
using IdentityServer.Application.DTOs;
using IdentityServer.Application.Interfaces;
using MediatR;

namespace IdentityServer.Application.Queries;

public class GetScopesQuery : IRequest<IEnumerable<ScopeDto>>
{
}
public class GetScopesQueryHandler : IRequestHandler<GetScopesQuery, IEnumerable<ScopeDto>>
{
    private readonly IScopeService _scopeService;
    private readonly IMapper _mapper;

    public GetScopesQueryHandler(IScopeService scopeService, IMapper mapper)
    {
        _scopeService = scopeService;
        _mapper = mapper;
    }

    public async Task<IEnumerable<ScopeDto>> Handle(GetScopesQuery request, CancellationToken cancellationToken)
    {
        var scopes = await _scopeService.GetAllScopesAsync();
        return _mapper.Map<IEnumerable<ScopeDto>>(scopes);
    }
}
