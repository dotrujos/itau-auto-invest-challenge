using Itau.AutoInvest.Application.Abstractions;
using Itau.AutoInvest.Application.UseCases.CustomerExit.IO;
using Itau.AutoInvest.Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace Itau.AutoInvest.Application.UseCases.CustomerExit.Implementations;

public class CustomerExitImpl : CustomerExit
{
    private readonly IClientRepository _clientRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CustomerExitImpl> _logger;

    public CustomerExitImpl(IClientRepository clientRepository, IUnitOfWork unitOfWork, ILogger<CustomerExitImpl> logger)
    {
        _clientRepository = clientRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    protected override async Task<CustomerExitOutput> ApplyInternalLogicAsync(CustomerExitInput input, CancellationToken ct)
    {
        _logger.LogInformation("Iniciando processo de saída para o cliente: {ClientId}", input.ClientId);

        var client = await _clientRepository.GetByIdAsync(input.ClientId, ct);

        if (client == null)
        {
            _logger.LogWarning("Tentativa de encerramento para cliente inexistente: {ClientId}", input.ClientId);
            throw new ClientNotFoundException();
        }

        if (!client.IsActive)
        {
            _logger.LogWarning("Cliente {ClientId} já se encontra inativo.", input.ClientId);
            throw new ClientAlreadyInactiveException();
        }

        client.Deactivate();

        await _clientRepository.UpdateAsync(client, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        _logger.LogInformation("Cliente {ClientId} desativado com sucesso.", client.Id);

        return new CustomerExitOutput(
            client.Id,
            client.Name,
            client.IsActive,
            DateTime.UtcNow,
            "Adesao encerrada. Sua posicao em custodia foi mantida."
        );
    }
}
