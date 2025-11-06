using MediatR;
using AutoMapper;
using IdentityServer.Application.DTOs;
using IdentityServer.Application.Interfaces;
using IdentityServer.Domain.Entities;

namespace IdentityServer.Application.Commands;

public class UpdateClientCommand : IRequest<ClientDto>
{
    public UpdateClientDto ClientDto { get; set; } = null!;
}

public class UpdateClientCommandHandler : IRequestHandler<UpdateClientCommand, ClientDto>
{
    private readonly IClientService _clientService;
    private readonly IMapper _mapper;

    public UpdateClientCommandHandler(IClientService clientService, IMapper mapper)
    {
        _clientService = clientService;
        _mapper = mapper;
    }

    public async Task<ClientDto> Handle(UpdateClientCommand request, CancellationToken cancellationToken)
    {
        var client = _mapper.Map<Client>(request.ClientDto);
        var updatedClient = await _clientService.UpdateClientAsync(client);
        return _mapper.Map<ClientDto>(updatedClient);
    }
}