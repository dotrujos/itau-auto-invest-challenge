using Itau.AutoInvest.Application.Abstractions;
using Itau.AutoInvest.Application.UseCases.ExecuteManualPurchase.Implementations;
using Itau.AutoInvest.Application.UseCases.ExecuteManualPurchase.IO;
using Itau.AutoInvest.Domain.Entities;
using Itau.AutoInvest.Domain.Exceptions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Itau.AutoInvest.Tests.Application.UseCases
{
    public class ExecuteManualPurchaseTests
    {
        [Fact]
        public async Task Execute_WhenPurchaseAlreadyExecuted_ThrowsException()
        {
            // Arrange
            var (sut, mocks) = CreateSut();
            var input = new ExecuteManualPurchaseInput(DateTime.Now);
            mocks.BuyOrderRepository.Setup(r => r.HasOrdersForDateAsync(input.DataReferencia, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act & Assert
            await Assert.ThrowsAsync<PurchaseAlreadyExecutedException>(
                () => sut.ExecuteAsync(input, CancellationToken.None));
        }

        [Fact]
        public async Task Execute_WhenNoActiveBasket_ThrowsException()
        {
            // Arrange
            var (sut, mocks) = CreateSut();
            var input = new ExecuteManualPurchaseInput(DateTime.Now);
            mocks.BuyOrderRepository.Setup(r => r.HasOrdersForDateAsync(input.DataReferencia, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);
            mocks.BasketRepository.Setup(r => r.GetActiveBasketAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync((RecommendationBasket)null);

            // Act & Assert
            await Assert.ThrowsAsync<BasketNotFoundException>(
                () => sut.ExecuteAsync(input, CancellationToken.None));
        }

        [Fact]
        public async Task Execute_WhenNoActiveClients_ReturnsSuccessWithNoClients()
        {
            // Arrange
            var (sut, mocks) = CreateSut();
            var input = new ExecuteManualPurchaseInput(DateTime.Now);
            var basketItems = new List<BasketItem>
            {
                new (1, "PETR4", 20),
                new (1, "VALE3", 20),
                new (1, "ITUB4", 20),
                new (1, "BBDC4", 20),
                new (1, "ABEV3", 20)
            };
            var basket = new RecommendationBasket("Test Basket", basketItems);

            mocks.BuyOrderRepository.Setup(r => r.HasOrdersForDateAsync(input.DataReferencia, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);
            mocks.BasketRepository.Setup(r => r.GetActiveBasketAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(basket);
            mocks.ClientRepository.Setup(r => r.GetAllActiveAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<Client>());

            // Act
            var result = await sut.ExecuteAsync(input, CancellationToken.None);

            // Assert
            Assert.Equal(0, result.TotalClientes);
            Assert.Contains("Nenhum cliente ativo", result.Mensagem);
        }

        private (ExecuteManualPurchaseImpl, Mocks) CreateSut()
        {
            var mocks = new Mocks(
                new Mock<IBasketRepository>(),
                new Mock<IClientRepository>(),
                new Mock<IGraphicalAccountRepository>(),
                new Mock<ICustodyRepository>(),
                new Mock<IStockRepository>(),
                new Mock<IBuyOrderRepository>(),
                new Mock<IDistributionRepository>(),
                new Mock<IKafkaProducer>(),
                new Mock<IUnitOfWork>(),
                new Mock<ILogger<ExecuteManualPurchaseImpl>>()
            );

            var sut = new ExecuteManualPurchaseImpl(
                mocks.BasketRepository.Object,
                mocks.ClientRepository.Object,
                mocks.AccountRepository.Object,
                mocks.CustodyRepository.Object,
                mocks.StockRepository.Object,
                mocks.BuyOrderRepository.Object,
                mocks.DistributionRepository.Object,
                mocks.KafkaProducer.Object,
                mocks.UnitOfWork.Object,
                mocks.Logger.Object
            );

            return (sut, mocks);
        }

        private record Mocks(
            Mock<IBasketRepository> BasketRepository,
            Mock<IClientRepository> ClientRepository,
            Mock<IGraphicalAccountRepository> AccountRepository,
            Mock<ICustodyRepository> CustodyRepository,
            Mock<IStockRepository> StockRepository,
            Mock<IBuyOrderRepository> BuyOrderRepository,
            Mock<IDistributionRepository> DistributionRepository,
            Mock<IKafkaProducer> KafkaProducer,
            Mock<IUnitOfWork> UnitOfWork,
            Mock<ILogger<ExecuteManualPurchaseImpl>> Logger
        );
    }
}
