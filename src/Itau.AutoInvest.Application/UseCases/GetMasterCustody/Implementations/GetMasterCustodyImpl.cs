using Itau.AutoInvest.Application.Abstractions;
using Itau.AutoInvest.Application.UseCases.GetMasterCustody.IO;
using Itau.AutoInvest.Domain.Exceptions;

namespace Itau.AutoInvest.Application.UseCases.GetMasterCustody.Implementations;

public class GetMasterCustodyImpl : GetMasterCustody
{
    private readonly IGraphicalAccountRepository _accountRepository;
    private readonly ICustodyRepository _custodyRepository;
    private readonly IStockRepository _stockRepository;

    public GetMasterCustodyImpl(
        IGraphicalAccountRepository accountRepository, 
        ICustodyRepository custodyRepository, 
        IStockRepository stockRepository)
    {
        _accountRepository = accountRepository;
        _custodyRepository = custodyRepository;
        _stockRepository = stockRepository;
    }

    protected override async Task<GetMasterCustodyOutput> ApplyInternalLogicAsync(CancellationToken ct)
    {
        var masterAccount = await _accountRepository.GetMasterAccountAsync(ct);

        if (masterAccount == null)
            throw new MasterAccountNotFoundException();

        var custodyItems = await _custodyRepository.GetByAccountIdAsync(masterAccount.Id, ct);
        
        var outputItems = new List<CustodiaMasterOutput>();
        decimal valorTotalResiduo = 0;

        foreach (var item in custodyItems)
        {
            var quote = await _stockRepository.GetLatestQuoteAsync(item.Ticker, ct);
            var cotacaoAtual = quote?.ClosingPrice ?? item.AveragePrice;
            var valorAtual = cotacaoAtual * item.Quantity;
            
            outputItems.Add(new CustodiaMasterOutput(
                item.Ticker,
                item.Quantity,
                item.AveragePrice,
                cotacaoAtual,
                "Resíduo de distribuição"
            ));

            valorTotalResiduo += valorAtual;
        }

        return new GetMasterCustodyOutput(
            new ContaMasterOutput(masterAccount.Id, masterAccount.AccountNumber, "MASTER"),
            outputItems,
            valorTotalResiduo
        );
    }
}
