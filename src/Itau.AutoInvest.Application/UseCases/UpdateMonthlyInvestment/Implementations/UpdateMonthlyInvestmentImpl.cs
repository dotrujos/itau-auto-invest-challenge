using Itau.AutoInvest.Application.Abstractions;
using Itau.AutoInvest.Application.UseCases.UpdateMonthlyInvestment.IO;
using Itau.AutoInvest.Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace Itau.AutoInvest.Application.UseCases.UpdateMonthlyInvestment.Implementations;

public class UpdateMonthlyInvestmentImpl : UpdateMonthlyInvestment
{
    private readonly IClientRepository _clientRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UpdateMonthlyInvestmentImpl> _logger;

    public UpdateMonthlyInvestmentImpl(IClientRepository clientRepository, IUnitOfWork unitOfWork, ILogger<UpdateMonthlyInvestmentImpl> logger)
    {
        _clientRepository = clientRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    protected override async Task<UpdateMonthlyInvestmentOutput> ApplyInternalLogicAsync(UpdateMonthlyInvestmentInput input, CancellationToken ct)
    {
        _logger.LogInformation("Iniciando atualização de aporte mensal para o cliente: {ClientId}, Novo Valor: {NovoValor}", input.ClientId, input.NewMonthlyValue);

        var client = await _clientRepository.GetByIdAsync(input.ClientId, ct);

        if (client == null)
        {
            _logger.LogWarning("Tentativa de atualização de aporte para cliente inexistente: {ClientId}", input.ClientId);
            throw new ClientNotFoundException();
        }

        decimal previousValue = client.MonthlyInvestment;
        
        client.UpdateMonthlyInvestment(input.NewMonthlyValue);

        await _clientRepository.UpdateAsync(client, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        _logger.LogInformation("Valor mensal do cliente {ClientId} atualizado de {ValorAnterior} para {ValorNovo}.", client.Id, previousValue, client.MonthlyInvestment);

        return new UpdateMonthlyInvestmentOutput(
            client.Id,
            previousValue,
            client.MonthlyInvestment,
            DateTime.UtcNow,
            "Valor mensal atualizado. O novo valor sera considerado a partir da proxima data de compra."
        );
    }
}
