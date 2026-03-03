using Itau.AutoInvest.Application.Abstractions;
using Itau.AutoInvest.Application.UseCases.ExecuteManualPurchase.IO;
using Itau.AutoInvest.Domain.Entities;
using Itau.AutoInvest.Domain.Enums;
using Itau.AutoInvest.Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace Itau.AutoInvest.Application.UseCases.ExecuteManualPurchase.Implementations;

public class ExecuteManualPurchaseImpl : ExecuteManualPurchase
{
    private readonly IBasketRepository _basketRepository;
    private readonly IClientRepository _clientRepository;
    private readonly IGraphicalAccountRepository _accountRepository;
    private readonly ICustodyRepository _custodyRepository;
    private readonly IStockRepository _stockRepository;
    private readonly IBuyOrderRepository _buyOrderRepository;
    private readonly IDistributionRepository _distributionRepository;
    private readonly IKafkaProducer _kafkaProducer;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ExecuteManualPurchaseImpl> _logger;

    public ExecuteManualPurchaseImpl(
        IBasketRepository basketRepository,
        IClientRepository clientRepository,
        IGraphicalAccountRepository accountRepository,
        ICustodyRepository custodyRepository,
        IStockRepository stockRepository,
        IBuyOrderRepository buyOrderRepository,
        IDistributionRepository distributionRepository,
        IKafkaProducer kafkaProducer,
        IUnitOfWork unitOfWork,
        ILogger<ExecuteManualPurchaseImpl> logger)
    {
        _basketRepository = basketRepository;
        _clientRepository = clientRepository;
        _accountRepository = accountRepository;
        _custodyRepository = custodyRepository;
        _stockRepository = stockRepository;
        _buyOrderRepository = buyOrderRepository;
        _distributionRepository = distributionRepository;
        _kafkaProducer = kafkaProducer;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    protected override async Task<ExecuteManualPurchaseOutput> ApplyInternalLogicAsync(ExecuteManualPurchaseInput input, CancellationToken ct)
    {
        if (await _buyOrderRepository.HasOrdersForDateAsync(input.DataReferencia, ct))
            throw new PurchaseAlreadyExecutedException(input.DataReferencia);

        var activeBasket = await _basketRepository.GetActiveBasketAsync(ct)
            ?? throw new BasketNotFoundException();

        var activeClients = (await _clientRepository.GetAllActiveAsync(ct)).ToList();
        if (!activeClients.Any())
            return CreateEmptyOutput();

        var masterAccount = await _accountRepository.GetMasterAccountAsync(ct)
            ?? throw new MasterAccountNotFoundException();

        await _unitOfWork.BeginTransactionAsync(ct);

        try
        {
            var clientesAporte = CalculateAportes(activeClients);
            decimal totalConsolidado = clientesAporte.Sum(x => x.Aporte);

            var purchaseResult = await ProcessConsolidatedPurchase(activeBasket, masterAccount, totalConsolidado, input.DataReferencia, ct);
            
            var distributionResult = await DistributeAssetsToClients(
                activeClients, 
                clientesAporte, 
                activeBasket, 
                masterAccount, 
                purchaseResult.Qtds, 
                purchaseResult.Cotacoes, 
                purchaseResult.OrdensIds, 
                totalConsolidado, 
                ct);

            var residuosMaster = await GetMasterResiduals(masterAccount.Id, ct);

            await _unitOfWork.CommitAsync(ct);

            return new ExecuteManualPurchaseOutput(
                DateTime.UtcNow,
                activeClients.Count,
                totalConsolidado,
                purchaseResult.Ordens,
                distributionResult.Distribuicoes,
                residuosMaster,
                distributionResult.EventosIR,
                $"Compra programada executada com sucesso para {activeClients.Count} clientes.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro fatal no motor de compra programada.");
            await _unitOfWork.RollbackAsync(ct);
            throw;
        }
    }

    private ExecuteManualPurchaseOutput CreateEmptyOutput()
    {
        return new ExecuteManualPurchaseOutput(DateTime.UtcNow, 0, 0, [], [], [], 0, "Nenhum cliente ativo para processar.");
    }

    private List<ClienteAporte> CalculateAportes(List<Client> clients)
    {
        return clients.Select(c => new ClienteAporte(
            c, 
            Math.Round(c.MonthlyInvestment / 3m, 2)
        )).ToList();
    }

    private async Task<(List<OrdemCompraOutput> Ordens, Dictionary<string, int> Qtds, Dictionary<string, decimal> Cotacoes, Dictionary<string, long> OrdensIds)> ProcessConsolidatedPurchase(
        RecommendationBasket basket, 
        GraphicalAccount masterAccount, 
        decimal totalConsolidado, 
        DateTime dataRef, 
        CancellationToken ct)
    {
        var ordens = new List<OrdemCompraOutput>();
        var qtds = new Dictionary<string, int>();
        var cotacoes = new Dictionary<string, decimal>();
        var ordensIds = new Dictionary<string, long>();

        foreach (var item in basket.Items)
        {
            var quote = await _stockRepository.GetLatestQuoteAsync(item.Ticker, ct)
                ?? throw new QuoteNotFoundException(dataRef);

            decimal valorParaAtivo = totalConsolidado * (item.Percentage / 100m);
            int quantidadeCalculada = (int)(valorParaAtivo / quote.ClosingPrice);
            
            var masterCustody = await _custodyRepository.GetByTickerAsync(masterAccount.Id, item.Ticker, ct);
            int residualMaster = masterCustody?.Quantity ?? 0;
            
            int quantidadeAComprar = Math.Max(0, quantidadeCalculada - residualMaster);
            
            qtds[item.Ticker] = quantidadeCalculada;
            cotacoes[item.Ticker] = quote.ClosingPrice;

            if (quantidadeAComprar > 0)
            {
                var detalhes = CreateOrderDetails(item.Ticker, quantidadeAComprar);
                ordens.Add(new OrdemCompraOutput(item.Ticker, quantidadeAComprar, detalhes, quote.ClosingPrice, quantidadeAComprar * quote.ClosingPrice));
                
                var buyOrder = new BuyOrder(masterAccount.Id, item.Ticker, quantidadeAComprar, quote.ClosingPrice, quantidadeAComprar >= 100 ? MarketType.Lote : MarketType.Fracionario, dataRef);
                await _buyOrderRepository.AddAsync(buyOrder, ct);
                ordensIds[item.Ticker] = buyOrder.Id;

                await UpdateMasterCustodyAsync(masterAccount.Id, masterCustody, item.Ticker, quantidadeAComprar, quote.ClosingPrice, ct);
            }
            else
            {
                ordensIds[item.Ticker] = 0; 
            }
        }
        return (ordens, qtds, cotacoes, ordensIds);
    }

    private async Task UpdateMasterCustodyAsync(long masterAccountId, Custody? currentCustody, string ticker, int quantity, decimal price, CancellationToken ct)
    {
        if (currentCustody == null)
        {
            await _custodyRepository.AddAsync(new Custody(masterAccountId, ticker, quantity, price), ct);
        }
        else
        {
            currentCustody.AddToPosition(quantity, price);
            await _custodyRepository.UpdateAsync(currentCustody, ct);
        }
    }

    private List<DetalheOrdemOutput> CreateOrderDetails(string ticker, int quantity)
    {
        var detalhes = new List<DetalheOrdemOutput>();
        int lotesPadrao = (quantity / 100) * 100;
        int fracionario = quantity % 100;

        if (lotesPadrao > 0) detalhes.Add(new DetalheOrdemOutput("LOTE", ticker, lotesPadrao));
        if (fracionario > 0) detalhes.Add(new DetalheOrdemOutput("FRACIONARIO", $"{ticker}F", fracionario));
        
        return detalhes;
    }

    private async Task<(List<DistribuicaoOutput> Distribuicoes, int EventosIR)> DistributeAssetsToClients(
        List<Client> clients, 
        List<ClienteAporte> aportes, 
        RecommendationBasket basket, 
        GraphicalAccount masterAccount, 
        Dictionary<string, int> qtdsPorTicker, 
        Dictionary<string, decimal> cotacoesPorTicker, 
        Dictionary<string, long> ordensIdsPorTicker,
        decimal totalConsolidado, 
        CancellationToken ct)
    {
        var distribuicoes = new List<DistribuicaoOutput>();
        int eventosIR = 0;

        foreach (var aporte in aportes)
        {
            var ativosDistribuidos = new List<AtivoDistribuidoOutput>();
            var clienteAccount = await _accountRepository.GetByClientIdAsync(aporte.Client.Id, ct);
            if (clienteAccount == null) continue;

            foreach (var itemCesta in basket.Items)
            {
                decimal proporcaoCliente = totalConsolidado > 0 ? aporte.Aporte / totalConsolidado : 0;
                int qtdParaCliente = (int)(qtdsPorTicker[itemCesta.Ticker] * proporcaoCliente);

                if (qtdParaCliente > 0)
                {
                    await TransferFromMasterToClient(masterAccount.Id, clienteAccount.Id, itemCesta.Ticker, qtdParaCliente, cotacoesPorTicker[itemCesta.Ticker], ct);
                    
                    var orderId = ordensIdsPorTicker[itemCesta.Ticker];
                    
                    if (orderId == 0)
                    {
                        orderId = await _buyOrderRepository.GetLatestOrderIdByTickerAsync(itemCesta.Ticker, ct);
                    }

                    if (orderId > 0)
                    {
                        await RecordDistributionAsync(clienteAccount.Id, orderId, itemCesta.Ticker, qtdParaCliente, cotacoesPorTicker[itemCesta.Ticker], ct);
                    }

                    ativosDistribuidos.Add(new AtivoDistribuidoOutput(itemCesta.Ticker, qtdParaCliente));
                    await PublishIRDedoDuro(aporte.Client, itemCesta.Ticker, qtdParaCliente, cotacoesPorTicker[itemCesta.Ticker], ct);
                    eventosIR++;
                }
            }
            distribuicoes.Add(new DistribuicaoOutput(aporte.Client.Id, aporte.Client.Name, aporte.Aporte, ativosDistribuidos));
        }
        return (distribuicoes, eventosIR);
    }

    private async Task RecordDistributionAsync(long accountId, long orderId, string ticker, int quantity, decimal price, CancellationToken ct)
    {
        var clientCustody = await _custodyRepository.GetByTickerAsync(accountId, ticker, ct);
        if (clientCustody != null)
        {
            var distributionRecord = new Distribution(orderId, clientCustody.Id, ticker, quantity, price);
            await _distributionRepository.AddAsync(distributionRecord, ct);
        }
    }

    private async Task TransferFromMasterToClient(long masterAccountId, long clientAccountId, string ticker, int quantity, decimal price, CancellationToken ct)
    {
        var masterCustody = await _custodyRepository.GetByTickerAsync(masterAccountId, ticker, ct);
        masterCustody?.RemoveFromPosition(quantity);
        await _custodyRepository.UpdateAsync(masterCustody!, ct);
        
        var clientCustody = await _custodyRepository.GetByTickerAsync(clientAccountId, ticker, ct);
        if (clientCustody == null)
        {
            await _custodyRepository.AddAsync(new Custody(clientAccountId, ticker, quantity, price), ct);
        }
        else
        {
            clientCustody.AddToPosition(quantity, price);
            await _custodyRepository.UpdateAsync(clientCustody, ct);
        }
    }

    private async Task PublishIRDedoDuro(Client client, string ticker, int quantity, decimal price, CancellationToken ct)
    {
        decimal valorOperacao = quantity * price;
        decimal irDedoDuro = valorOperacao * 0.00005m; 

        var kafkaMsg = new {
            ClienteId = client.Id,
            CPF = client.Cpf.Number,
            Ticker = ticker,
            ValorOperacao = valorOperacao,
            ValorIR = irDedoDuro,
            Data = DateTime.UtcNow
        };

        await _kafkaProducer.PublishAsync("ir-dedo-duro", client.Id.ToString(), kafkaMsg, ct);
    }

    private async Task<List<ResiduoMasterOutput>> GetMasterResiduals(long masterAccountId, CancellationToken ct)
    {
        var allMasterCustody = await _custodyRepository.GetByAccountIdAsync(masterAccountId, ct);
        return allMasterCustody
            .Where(c => c.Quantity > 0)
            .Select(res => new ResiduoMasterOutput(res.Ticker, res.Quantity))
            .ToList();
    }

    private record ClienteAporte(Client Client, decimal Aporte);
}
