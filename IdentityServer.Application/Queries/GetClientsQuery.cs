using MediatR;
using AutoMapper;
using IdentityServer.Application.DTOs;
using IdentityServer.Application.Interfaces;

namespace IdentityServer.Application.Queries;

public class GetClientsQuery : IRequest<IEnumerable<ClientDto>>
{
}

public class GetClientsQueryHandler : IRequestHandler<GetClientsQuery, IEnumerable<ClientDto>>
{
    private readonly IClientService _clientService;
    private readonly IMapper _mapper;

    public GetClientsQueryHandler(IClientService clientService, IMapper mapper)
    {
        _clientService = clientService;
        _mapper = mapper;
    }

    public async Task<IEnumerable<ClientDto>> Handle(GetClientsQuery request, CancellationToken cancellationToken)
    {
        var clients = await _clientService.GetAllClientsAsync();
        return _mapper.Map<IEnumerable<ClientDto>>(clients);
    }
}