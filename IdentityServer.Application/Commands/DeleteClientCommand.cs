using MediatR;
using IdentityServer.Application.Interfaces;

namespace IdentityServer.Application.Commands;

public class DeleteClientCommand : IRequest<bool>
{
    public int Id { get; set; }
}

public class DeleteClientCommandHandler : IRequestHandler<DeleteClientCommand, bool>
{
    private readonly IClientService _clientService;

    public DeleteClientCommandHandler(IClientService clientService)
    {
        _clientService = clientService;
    }

    public async Task<bool> Handle(DeleteClientCommand request, CancellationToken cancellationToken)
    {
        return await _clientService.DeleteClientAsync(request.Id);
    }
}