using Itau.AutoInvest.Application.Abstractions;
using Itau.AutoInvest.Application.UseCases.UpdateMonthlyInvestment.IO;
using Itau.AutoInvest.Domain.Exceptions;

namespace Itau.AutoInvest.Application.UseCases.UpdateMonthlyInvestment.Implementations;

public class UpdateMonthlyInvestmentImpl : UpdateMonthlyInvestment
{
    private readonly IClientRepository _clientRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateMonthlyInvestmentImpl(IClientRepository clientRepository, IUnitOfWork unitOfWork)
    {
        _clientRepository = clientRepository;
        _unitOfWork = unitOfWork;
    }

    protected override async Task<UpdateMonthlyInvestmentOutput> ApplyInternalLogicAsync(UpdateMonthlyInvestmentInput input, CancellationToken ct)
    {
        var client = await _clientRepository.GetByIdAsync(input.ClientId, ct);

        if (client == null)
        {
            throw new ClientNotFoundException();
        }

        decimal previousValue = client.MonthlyInvestment;
        
        client.UpdateMonthlyInvestment(input.NewMonthlyValue);

        await _clientRepository.UpdateAsync(client, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        return new UpdateMonthlyInvestmentOutput(
            client.Id,
            previousValue,
            client.MonthlyInvestment,
            DateTime.UtcNow,
            "Valor mensal atualizado. O novo valor sera considerado a partir da proxima data de compra."
        );
    }
}
