using System.Text;
using Itau.AutoInvest.Application.Abstractions;
using Itau.AutoInvest.Application.Jobs.CotahistIngestion;
using Itau.AutoInvest.Domain.Entities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;

namespace Itau.AutoInvest.Tests.Application.Jobs;

public class CotahistIngestionJobTests
{
    private readonly Mock<IServiceScopeFactory> _scopeFactoryMock;
    private readonly Mock<IServiceScope> _scopeMock;
    private readonly Mock<IServiceProvider> _serviceProviderMock;
    private readonly Mock<IFileExplorer> _fileExplorerMock;
    private readonly Mock<IStockRepository> _stockRepoMock;
    private readonly Mock<ILogger<CotahistIngestionJob>> _loggerMock;
    private readonly CotahistIngestionJob _job;

    public CotahistIngestionJobTests()
    {
        _scopeFactoryMock = new Mock<IServiceScopeFactory>();
        _scopeMock = new Mock<IServiceScope>();
        _serviceProviderMock = new Mock<IServiceProvider>();
        _fileExplorerMock = new Mock<IFileExplorer>();
        _stockRepoMock = new Mock<IStockRepository>();
        _loggerMock = new Mock<ILogger<CotahistIngestionJob>>();

        _scopeFactoryMock.Setup(f => f.CreateScope()).Returns(_scopeMock.Object);
        _scopeMock.Setup(s => s.ServiceProvider).Returns(_serviceProviderMock.Object);
        
        _serviceProviderMock.Setup(sp => sp.GetService(typeof(IFileExplorer))).Returns(_fileExplorerMock.Object);
        _serviceProviderMock.Setup(sp => sp.GetService(typeof(IStockRepository))).Returns(_stockRepoMock.Object);

        _job = new CotahistIngestionJob(_scopeFactoryMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task ExecuteAsync_WithPendingFiles_ShouldProcessAndSaveStocks()
    {
        // Arrange
        var tempFilePath = Path.GetTempFileName();
        try
        {
            var date = "20260301";
            var ticker = "PETR4".PadRight(12);
            var opening = "0000000003400";
            var max = "0000000003600";
            var min = "0000000003300";
            var other = new string(' ', 13);
            var closing = "0000000003500";
            var line = $"01{date}  {ticker}{new string(' ', 32)}{opening}{max}{min}{other}{closing}";

            await File.WriteAllLinesAsync(tempFilePath, new[] { line });

            var fileInfo = new FileInfo(tempFilePath);
            _fileExplorerMock.Setup(f => f.GetPendingFiles()).Returns(new List<FileInfo> { fileInfo });

            using var cts = new CancellationTokenSource();
            
            // Act
            // Start the job in a separate task
            var jobTask = _job.StartAsync(cts.Token);
            
            // Wait for a bit for the job to process the file
            await Task.Delay(500);
            
            // Cancel and wait for job to finish
            cts.Cancel();
            await jobTask;

            // Assert
            _stockRepoMock.Verify(r => r.SaveAsync(It.IsAny<StockQuote>(), It.IsAny<CancellationToken>()), Times.AtLeastOnce());
            Assert.False(File.Exists(tempFilePath), "File should be deleted after processing.");
        }
        finally
        {
            if (File.Exists(tempFilePath))
                File.Delete(tempFilePath);
        }
    }

    [Fact]
    public async Task ExecuteAsync_WithNoPendingFiles_ShouldDoNothing()
    {
        // Arrange
        _fileExplorerMock.Setup(f => f.GetPendingFiles()).Returns(new List<FileInfo>());

        using var cts = new CancellationTokenSource();
        
        // Act
        var jobTask = _job.StartAsync(cts.Token);
        await Task.Delay(200);
        cts.Cancel();
        await jobTask;

        // Assert
        _stockRepoMock.Verify(r => r.SaveAsync(It.IsAny<StockQuote>(), It.IsAny<CancellationToken>()), Times.Never());
    }
}
