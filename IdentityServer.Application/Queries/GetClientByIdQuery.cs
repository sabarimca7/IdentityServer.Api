using MediatR;
using AutoMapper;
using IdentityServer.Application.DTOs;
using IdentityServer.Application.Interfaces;

namespace IdentityServer.Application.Queries;

public class GetClientByIdQuery : IRequest<ClientDto?>
{
    public int Id { get; set; }
}

public class GetClientByIdQueryHandler : IRequestHandler<GetClientByIdQuery, ClientDto?>
{
    private readonly IClientService _clientService;
    private readonly IMapper _mapper;

    public GetClientByIdQueryHandler(IClientService clientService, IMapper mapper)
    {
        _clientService = clientService;
        _mapper = mapper;
    }

    public async Task<ClientDto?> Handle(GetClientByIdQuery request, CancellationToken cancellationToken)
    {
        var client = await _clientService.GetClientByIdAsync(request.Id);
        return client != null ? _mapper.Map<ClientDto>(client) : null;
    }
}