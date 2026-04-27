using AutoMapper;
using IdentityServer.Application.DTOs;
using IdentityServer.Application.Interfaces;
using MediatR;

namespace IdentityServer.Application.Queries;

public class GetScopeByIdQuery : IRequest<ScopeDto?>
{
    public int Id { get; set; }
}
public class GetScopeByIdQueryHandler : IRequestHandler<GetScopeByIdQuery, ScopeDto>
{
    private readonly IScopeService _scopeService;
    private readonly IMapper _mapper;

    public GetScopeByIdQueryHandler(IScopeService scopeService, IMapper mapper)
    {
        _scopeService = scopeService;
        _mapper = mapper;
    }

    public async Task<ScopeDto> Handle(GetScopeByIdQuery request, CancellationToken cancellationToken)
    {
        var scope = await _scopeService.GetScopeByIdAsync(request.Id);

        if (scope == null) return null;

        return _mapper.Map<ScopeDto>(scope); // ← is Scope entity being passed here?
    }
}