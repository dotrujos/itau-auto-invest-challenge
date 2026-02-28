using Itau.AutoInvest.Application.Abstractions;
using Itau.AutoInvest.Application.UseCases.CustomerExit.IO;
using Itau.AutoInvest.Domain.Exceptions;

namespace Itau.AutoInvest.Application.UseCases.CustomerExit.Implementations;

public class CustomerExitImpl : CustomerExit
{
    private readonly IClientRepository _clientRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CustomerExitImpl(IClientRepository clientRepository, IUnitOfWork unitOfWork)
    {
        _clientRepository = clientRepository;
        _unitOfWork = unitOfWork;
    }

    protected override async Task<CustomerExitOutput> ApplyInternalLogicAsync(CustomerExitInput input, CancellationToken ct)
    {
        var client = await _clientRepository.GetByIdAsync(input.ClientId, ct);

        if (client == null)
        {
            throw new ClientNotFoundException();
        }

        if (!client.IsActive)
        {
            throw new ClientAlreadyInactiveException();
        }

        client.Deactivate();

        await _clientRepository.UpdateAsync(client, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        return new CustomerExitOutput(
            client.Id,
            client.Name,
            client.IsActive,
            DateTime.UtcNow,
            "Adesao encerrada. Sua posicao em custodia foi mantida."
        );
    }
}
