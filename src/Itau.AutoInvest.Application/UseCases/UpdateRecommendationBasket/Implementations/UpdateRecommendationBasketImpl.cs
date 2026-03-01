using Itau.AutoInvest.Application.Abstractions;
using Itau.AutoInvest.Application.UseCases.UpdateRecommendationBasket.IO;
using Itau.AutoInvest.Domain.Entities;
using Itau.AutoInvest.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace Itau.AutoInvest.Application.UseCases.UpdateRecommendationBasket.Implementations;

public class UpdateRecommendationBasketImpl : UpdateRecommendationBasket
{
    private readonly IBasketRepository _basketRepository;
    private readonly IClientRepository _clientRepository;
    private readonly ICustodyRepository _custodyRepository;
    private readonly IGraphicalAccountRepository _accountRepository;
    private readonly IStockRepository _stockRepository;
    private readonly IRebalanceRepository _rebalanceRepository;
    private readonly IIREventRepository _irEventRepository;
    private readonly IKafkaProducer _kafkaProducer;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UpdateRecommendationBasketImpl> _logger;

    public UpdateRecommendationBasketImpl(
        IBasketRepository basketRepository, 
        IClientRepository clientRepository,
        ICustodyRepository custodyRepository,
        IGraphicalAccountRepository accountRepository,
        IStockRepository stockRepository,
        IRebalanceRepository rebalanceRepository,
        IIREventRepository irEventRepository,
        IKafkaProducer kafkaProducer,
        IUnitOfWork unitOfWork,
        ILogger<UpdateRecommendationBasketImpl> logger)
    {
        _basketRepository = basketRepository;
        _clientRepository = clientRepository;
        _custodyRepository = custodyRepository;
        _accountRepository = accountRepository;
        _stockRepository = stockRepository;
        _rebalanceRepository = rebalanceRepository;
        _irEventRepository = irEventRepository;
        _kafkaProducer = kafkaProducer;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    protected override async Task<UpdateRecommendationBasketOutput> ApplyInternalLogicAsync(UpdateRecommendationBasketInput input, CancellationToken ct)
    {
        await _unitOfWork.BeginTransactionAsync(ct);

        try
        {
            var activeBasket = await _basketRepository.GetActiveBasketAsync(ct);
            var removedAssets = new List<string>();
            var addedAssets = new List<string>();
            DeactivatedBasketInfo? deactivatedInfo = null;

            if (activeBasket != null)
            {
                var oldTickers = activeBasket.Items.Select(i => i.Ticker).ToList();
                var newTickers = input.Items.Select(i => i.Ticker).ToList();

                removedAssets = oldTickers.Except(newTickers).ToList();
                addedAssets = newTickers.Except(oldTickers).ToList();

                activeBasket.Deactivate();
                await _basketRepository.UpdateAsync(activeBasket, ct);
                
                deactivatedInfo = new DeactivatedBasketInfo(
                    activeBasket.Id,
                    activeBasket.Name,
                    activeBasket.DeactivationDate
                );
            }

            var newItems = input.Items.Select(i => new BasketItem(i.Ticker, i.Percentage)).ToList();
            var newBasket = new RecommendationBasket(input.Name, newItems);

            await _basketRepository.AddAsync(newBasket, ct);
            
            if (activeBasket != null && removedAssets.Any())
            {
                await ProcessRebalancingAsync(removedAssets, ct);
            }

            await _unitOfWork.CommitAsync(ct);

            bool rebalancingTriggered = activeBasket != null;
            string message = activeBasket == null 
                ? "Primeira cesta cadastrada com sucesso." 
                : $"Cesta atualizada. Rebalanceamento disparado para os clientes ativos.";

            return new UpdateRecommendationBasketOutput(
                newBasket.Id,
                newBasket.Name,
                newBasket.IsActive,
                newBasket.CreatedAt,
                input.Items,
                rebalancingTriggered,
                message,
                deactivatedInfo,
                removedAssets.Any() ? removedAssets : null,
                addedAssets.Any() ? addedAssets : null
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao atualizar cesta de recomendação.");
            await _unitOfWork.RollbackAsync(ct);
            throw;
        }
    }

    private async Task ProcessRebalancingAsync(List<string> removedAssets, CancellationToken ct)
    {
        var activeClients = await _clientRepository.GetAllActiveAsync(ct);
        var now = DateTime.UtcNow;
        
        foreach (var client in activeClients)
        {
            var account = await _accountRepository.GetByClientIdAsync(client.Id, ct);
            if (account == null) continue;

            decimal totalVendasNoMesAnterior = await _rebalanceRepository.GetTotalSalesInMonthAsync(client.Id, now.Month, now.Year, ct);
            decimal totalVendasRebalanceamento = 0;
            decimal lucroTotalRebalanceamento = 0;

            foreach (var ticker in removedAssets)
            {
                var custody = await _custodyRepository.GetByTickerAsync(account.Id, ticker, ct);
                if (custody == null || custody.Quantity <= 0) continue;

                var quote = await _stockRepository.GetLatestQuoteAsync(ticker, ct);
                decimal currentPrice = quote?.ClosingPrice ?? custody.AveragePrice;
                decimal salesValue = custody.Quantity * currentPrice;
                decimal costValue = custody.Quantity * custody.AveragePrice;
                decimal profit = salesValue - costValue;
                
                var rebalance = new Rebalance(client.Id, RebalanceType.Mudanca_Cesta, ticker, "CASH_PENDING", salesValue);
                await _rebalanceRepository.AddAsync(rebalance, ct);

                // Atualizar custódia (venda total do ativo removido)
                custody.RemoveFromPosition(custody.Quantity);
                await _custodyRepository.UpdateAsync(custody, ct);

                totalVendasRebalanceamento += salesValue;
                lucroTotalRebalanceamento += profit;
            }
            
            if (totalVendasNoMesAnterior + totalVendasRebalanceamento > 20000 && lucroTotalRebalanceamento > 0)
            {
                decimal irValue = lucroTotalRebalanceamento * 0.20m;
                var irEvent = new IREvent(client.Id, IREventType.IR_Venda, lucroTotalRebalanceamento, irValue);
                await _irEventRepository.AddAsync(irEvent, ct);
                
                var kafkaMessage = new
                {
                    ClienteId = client.Id,
                    CPF = client.Cpf.Number,
                    TipoEvento = "IR_VENDA_REBALANCEAMENTO",
                    ValorOperacao = totalVendasRebalanceamento,
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
                    _logger.LogWarning(ex, "Falha ao publicar IR de venda no Kafka para o cliente {ClientId}. O evento será mantido como não publicado para reprocessamento.", client.Id);
                }
            }
        }
    }
}
