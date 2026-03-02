using System;
using System.Threading;
using System.Threading.Tasks;
using Itau.AutoInvest.Application.Abstractions;
using Itau.AutoInvest.Application.Jobs.PurchaseScheduler;
using Itau.AutoInvest.Application.UseCases.ExecuteManualPurchase;
using Itau.AutoInvest.Application.UseCases.ExecuteManualPurchase.IO;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Itau.AutoInvest.Tests.Integration.Jobs
{
    public class PurchaseSchedulerJobTests
    {
        [Theory]
        [InlineData(2026, 3, 5, true)] // Wednesday
        [InlineData(2026, 3, 6, false)] // Not an execution day
        [InlineData(2026, 3, 7, false)] // Saturday
        public async Task Job_TriggersOnCorrectDays(int year, int month, int day, bool shouldTrigger)
        {
            // Arrange
            var today = new DateTime(year, month, day);
            var (sut, mocks) = CreateSut();
            
            mocks.BuyOrderRepository.Setup(r => r.HasOrdersForDateAsync(today, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            // Act
            await sut.StartAsync(CancellationToken.None); 
            await Task.Delay(100);

            // Assert
            var expectedTimes = shouldTrigger ? Times.Once() : Times.Never();
            mocks.ExecuteManualPurchase.Verify(u => u.ExecuteAsync(It.IsAny<ExecuteManualPurchaseInput>(), It.IsAny<CancellationToken>()), expectedTimes);
        }

        [Fact]
        public async Task Job_WhenPurchaseAlreadyExecuted_DoesNotTrigger()
        {
            // Arrange
            var today = new DateTime(2026, 3, 5); // Execution day
            var (sut, mocks) = CreateSut();
            
            mocks.BuyOrderRepository.Setup(r => r.HasOrdersForDateAsync(today, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);
            
            // Act
            await sut.StartAsync(CancellationToken.None);
            await Task.Delay(100);

            // Assert
            mocks.ExecuteManualPurchase.Verify(u => u.ExecuteAsync(It.IsAny<ExecuteManualPurchaseInput>(), It.IsAny<CancellationToken>()), Times.Never());
        }

        private (PurchaseSchedulerJob, Mocks) CreateSut()
        {
            var serviceProvider = new Mock<IServiceProvider>();
            var serviceScope = new Mock<IServiceScope>();
            var serviceScopeFactory = new Mock<IServiceScopeFactory>();
            var executeManualPurchase = new Mock<ExecuteManualPurchase>();
            var buyOrderRepository = new Mock<IBuyOrderRepository>();

            serviceProvider.Setup(p => p.GetService(typeof(IServiceScopeFactory))).Returns(serviceScopeFactory.Object);
            serviceScopeFactory.Setup(f => f.CreateScope()).Returns(serviceScope.Object);
            serviceScope.Setup(s => s.ServiceProvider.GetRequiredService(typeof(ExecuteManualPurchase))).Returns(executeManualPurchase.Object);
            serviceScope.Setup(s => s.ServiceProvider.GetRequiredService(typeof(IBuyOrderRepository))).Returns(buyOrderRepository.Object);

            var mocks = new Mocks(
                serviceProvider,
                executeManualPurchase,
                new Mock<IBuyOrderRepository>(),
                new Mock<ILogger<PurchaseSchedulerJob>>()
            );

            var sut = new PurchaseSchedulerJob(
                mocks.ServiceProvider.Object,
                mocks.Logger.Object
            );

            return (sut, mocks);
        }
        
        private record Mocks(
            Mock<IServiceProvider> ServiceProvider,
            Mock<ExecuteManualPurchase> ExecuteManualPurchase,
            Mock<IBuyOrderRepository> BuyOrderRepository,
            Mock<ILogger<PurchaseSchedulerJob>> Logger
        );
    }
}
