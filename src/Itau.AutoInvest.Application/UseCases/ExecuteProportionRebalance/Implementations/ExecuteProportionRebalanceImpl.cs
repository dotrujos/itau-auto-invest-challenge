using Itau.AutoInvest.Application.Abstractions;
using Itau.AutoInvest.Application.UseCases.ExecuteProportionRebalance.IO;
using Itau.AutoInvest.Domain.Entities;
using Itau.AutoInvest.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace Itau.AutoInvest.Application.UseCases.ExecuteProportionRebalance.Implementations;

public class ExecuteProportionRebalanceImpl : ExecuteProportionRebalance
{
    private readonly IBasketRepository _basketRepository;
    private readonly IClientRepository _clientRepository;
    private readonly IGraphicalAccountRepository _accountRepository;
    private readonly ICustodyRepository _custodyRepository;
    private readonly IStockRepository _stockRepository;
    private readonly IRebalanceRepository _rebalanceRepository;
    private readonly IIREventRepository _irEventRepository;
    private readonly IKafkaProducer _kafkaProducer;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ExecuteProportionRebalanceImpl> _logger;

    public ExecuteProportionRebalanceImpl(
        IBasketRepository basketRepository,
        IClientRepository clientRepository,
        IGraphicalAccountRepository accountRepository,
        ICustodyRepository custodyRepository,
        IStockRepository stockRepository,
        IRebalanceRepository rebalanceRepository,
        IIREventRepository irEventRepository,
        IKafkaProducer kafkaProducer,
        IUnitOfWork unitOfWork,
        ILogger<ExecuteProportionRebalanceImpl> logger)
    {
        _basketRepository = basketRepository;
        _clientRepository = clientRepository;
        _accountRepository = accountRepository;
        _custodyRepository = custodyRepository;
        _stockRepository = stockRepository;
        _rebalanceRepository = rebalanceRepository;
        _irEventRepository = irEventRepository;
        _kafkaProducer = kafkaProducer;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    protected override async Task<ExecuteProportionRebalanceOutput> ApplyInternalLogicAsync(ExecuteProportionRebalanceInput input, CancellationToken ct)
    {
        var activeBasket = await _basketRepository.GetActiveBasketAsync(ct);
        if (activeBasket == null)
            return new ExecuteProportionRebalanceOutput(DateTime.UtcNow, 0, 0, 0, "Nenhuma cesta ativa para validar desvios.");

        var activeClients = await _clientRepository.GetAllActiveAsync(ct);
        var enumerable = activeClients.ToList();
        if (!enumerable.Any())
            return new ExecuteProportionRebalanceOutput(DateTime.UtcNow, 0, 0, 0, "Nenhum cliente ativo para processar.");

        int clientsRebalanced = 0;
        decimal totalGlobalSales = 0;
        decimal totalGlobalInvested = 0;
        var now = DateTime.UtcNow;

        await _unitOfWork.BeginTransactionAsync(ct);

        try
        {
            foreach (var client in enumerable)
            {
                var account = await _accountRepository.GetByClientIdAsync(client.Id, ct);
                if (account == null) continue;

                var custody = await _custodyRepository.GetByAccountIdAsync(account.Id, ct);
                if (!custody.Any()) continue;

                // 1. Calcular Valor Total da Carteira
                decimal totalPortfolioValue = 0;
                var currentQuotes = new Dictionary<string, decimal>();

                foreach (var item in custody)
                {
                    var quote = await _stockRepository.GetLatestQuoteAsync(item.Ticker, ct);
                    decimal price = quote?.ClosingPrice ?? item.AveragePrice;
                    currentQuotes[item.Ticker] = price;
                    totalPortfolioValue += item.Quantity * price;
                }

                if (totalPortfolioValue == 0) continue;

                // 2. Avaliar Desvios
                bool needsRebalancing = false;
                decimal cashToReinvest = 0;
                decimal totalSalesRebalanceamento = 0;
                decimal lucroTotalRebalanceamento = 0;
                decimal limitThreshold = input.ThresholdPercentage / 100m;

                var underAllocatedAssets = new List<(string Ticker, decimal TargetValue, decimal CurrentValue, decimal CurrentPrice)>();

                foreach (var basketItem in activeBasket.Items)
                {
                    var custodyItem = custody.FirstOrDefault(c => c.Ticker == basketItem.Ticker);
                    decimal currentPrice = currentQuotes.ContainsKey(basketItem.Ticker) ? currentQuotes[basketItem.Ticker] : 0;
                    if (currentPrice == 0) continue;

                    decimal targetPercentage = basketItem.Percentage / 100m;
                    decimal targetValue = totalPortfolioValue * targetPercentage;
                    int currentQty = custodyItem?.Quantity ?? 0;
                    decimal currentValue = currentQty * currentPrice;

                    decimal currentPercentage = currentValue / totalPortfolioValue;
                    decimal deviation = currentPercentage - targetPercentage;

                    if (deviation > limitThreshold)
                    {
                        // Sobre-alocado: Vender excesso
                        int targetQty = (int)(targetValue / currentPrice);
                        int qtyToSell = currentQty - targetQty;

                        if (qtyToSell > 0)
                        {
                            decimal salesValue = qtyToSell * currentPrice;
                            decimal costValue = qtyToSell * custodyItem!.AveragePrice;
                            decimal profit = salesValue - costValue;

                            await ExecuteSale(client.Id, custodyItem, qtyToSell, currentPrice, ct);

                            totalSalesRebalanceamento += salesValue;
                            lucroTotalRebalanceamento += profit;
                            cashToReinvest += salesValue;
                            needsRebalancing = true;
                        }
                    }
                    else if (deviation < -limitThreshold || (currentPercentage < targetPercentage && currentQty == 0))
                    {
                        // Sub-alocado: Marcar para compra
                        underAllocatedAssets.Add((basketItem.Ticker, targetValue, currentValue, currentPrice));
                    }
                }

                // 3. Regras Fiscais de Venda
                if (totalSalesRebalanceamento > 0)
                {
                    decimal totalVendasNoMesAnterior = await _rebalanceRepository.GetTotalSalesInMonthAsync(client.Id, now.Month, now.Year, ct);
                    
                    if (totalVendasNoMesAnterior + totalSalesRebalanceamento > 20000 && lucroTotalRebalanceamento > 0)
                    {
                        await ProcessIRVendaAsync(client, totalSalesRebalanceamento, lucroTotalRebalanceamento, now, ct);
                    }
                }

                // 4. Reinvestir caixa nos sub-alocados
                if (cashToReinvest > 0 && underAllocatedAssets.Any())
                {
                    decimal totalMissingValue = underAllocatedAssets.Sum(x => Math.Max(0, x.TargetValue - x.CurrentValue));

                    foreach (var ua in underAllocatedAssets)
                    {
                        if (cashToReinvest <= 0) break;

                        decimal missingValue = Math.Max(0, ua.TargetValue - ua.CurrentValue);
                        decimal proportion = totalMissingValue > 0 ? (missingValue / totalMissingValue) : 0;
                        
                        decimal cashForAsset = Math.Min(cashToReinvest * proportion, cashToReinvest);
                        int qtyToBuy = (int)(cashForAsset / ua.CurrentPrice);

                        if (qtyToBuy > 0)
                        {
                            var cItem = custody.FirstOrDefault(c => c.Ticker == ua.Ticker);
                            await ExecuteBuy(client.Id, account.Id, cItem, ua.Ticker, qtyToBuy, ua.CurrentPrice, ct);
                            cashToReinvest -= qtyToBuy * ua.CurrentPrice;
                        }
                    }
                }

                if (needsRebalancing)
                {
                    clientsRebalanced++;
                    totalGlobalSales += totalSalesRebalanceamento;
                    totalGlobalInvested += cashToReinvest; // Sobras de caixa se houver, ou valor que foi realocado
                }
            }

            await _unitOfWork.CommitAsync(ct);

            return new ExecuteProportionRebalanceOutput(
                DateTime.UtcNow,
                clientsRebalanced,
                totalGlobalSales,
                totalGlobalSales - totalGlobalInvested, // Valor total investido é vendas menos sobras
                $"Rebalanceamento por desvio executado. {clientsRebalanced} clientes rebalanceados."
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro fatal no motor de rebalanceamento por desvio.");
            await _unitOfWork.RollbackAsync(ct);
            throw;
        }
    }

    private async Task ExecuteSale(long clientId, Custody custody, int quantity, decimal price, CancellationToken ct)
    {
        var rebalance = new Rebalance(clientId, RebalanceType.Desvio, custody.Ticker, "CASH_PENDING", quantity * price);
        await _rebalanceRepository.AddAsync(rebalance, ct);

        custody.RemoveFromPosition(quantity);
        await _custodyRepository.UpdateAsync(custody, ct);
    }

    private async Task ExecuteBuy(long clientId, long accountId, Custody? custody, string ticker, int quantity, decimal price, CancellationToken ct)
    {
        var rebalance = new Rebalance(clientId, RebalanceType.Desvio, "CASH_PENDING", ticker, quantity * price);
        await _rebalanceRepository.AddAsync(rebalance, ct);

        if (custody == null)
        {
            await _custodyRepository.AddAsync(new Custody(accountId, ticker, quantity, price), ct);
        }
        else
        {
            custody.AddToPosition(quantity, price);
            await _custodyRepository.UpdateAsync(custody, ct);
        }
    }

    private async Task ProcessIRVendaAsync(Client client, decimal totalVendas, decimal lucroTotal, DateTime now, CancellationToken ct)
    {
        decimal irValue = lucroTotal * 0.20m;
        var irEvent = new IREvent(client.Id, IREventType.IR_Venda, lucroTotal, irValue);
        await _irEventRepository.AddAsync(irEvent, ct);
        
        var kafkaMessage = new
        {
            ClienteId = client.Id,
            CPF = client.Cpf.Number,
            TipoEvento = "IR_VENDA_REBALANCEAMENTO_DESVIO",
            ValorOperacao = totalVendas,
            ValorIR = irValue,
            Data = now
        };

        try
        {
            await _kafkaProducer.PublishAsync("ir-venda", client.Id.ToString(), kafkaMessage, ct);
            irEvent.MarkAsPublished();
            await _irEventRepository.UpdateAsync(irEvent, ct);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Falha ao publicar IR de venda no Kafka para o cliente {ClientId}.", client.Id);
        }
    }
}
