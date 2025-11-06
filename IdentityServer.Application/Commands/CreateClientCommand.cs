using AutoMapper;
using IdentityServer.Application.DTOs;
using IdentityServer.Application.Interfaces;
using IdentityServer.Domain.Entities;
using MediatR;

namespace IdentityServer.Application.Commands;

public class CreateClientCommand : IRequest<ClientDto>
{
    public CreateClientDto ClientDto { get; set; } = null!;
}

public class CreateClientCommandHandler : IRequestHandler<CreateClientCommand, ClientDto>
{
    private readonly IClientService _clientService;
    private readonly IMapper _mapper;

    public CreateClientCommandHandler(IClientService clientService, IMapper mapper)
    {
        _clientService = clientService;
        _mapper = mapper;
    }

    public async Task<ClientDto> Handle(CreateClientCommand request, CancellationToken cancellationToken)
    {
        var client = _mapper.Map<Client>(request.ClientDto);
        client.CreatedOn = DateTime.UtcNow;

        var createdClient = await _clientService.CreateClientAsync(client);
        return _mapper.Map<ClientDto>(createdClient);
    }
}