using Itau.AutoInvest.Application.Abstractions;
using Itau.AutoInvest.Application.UseCases.UpdateRecommendationBasket.IO;
using Itau.AutoInvest.Domain.Entities;

namespace Itau.AutoInvest.Application.UseCases.UpdateRecommendationBasket.Implementations;

public class UpdateRecommendationBasketImpl : UpdateRecommendationBasket
{
    private readonly IBasketRepository _basketRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateRecommendationBasketImpl(IBasketRepository basketRepository, IUnitOfWork unitOfWork)
    {
        _basketRepository = basketRepository;
        _unitOfWork = unitOfWork;
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
        catch (Exception)
        {
            await _unitOfWork.RollbackAsync(ct);
            throw;
        }
    }
}
