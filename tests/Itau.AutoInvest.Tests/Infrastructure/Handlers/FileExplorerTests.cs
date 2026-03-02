using Itau.AutoInvest.Infrastructure.Handlers;
using Microsoft.Extensions.Configuration;
using Moq;

namespace Itau.AutoInvest.Tests.Infrastructure.Handlers;

public class FileExplorerTests : IDisposable
{
    private readonly string _testPath;
    private readonly Mock<IConfiguration> _configMock;

    public FileExplorerTests()
    {
        _testPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(_testPath);
        
        _configMock = new Mock<IConfiguration>();
        _configMock.Setup(c => c["CotacoesPath"]).Returns(_testPath);
    }

    [Fact]
    public void GetPendingFiles_WithMatchingFiles_ShouldReturnThemSorted()
    {
        // Arrange
        var file1 = Path.Combine(_testPath, "COTAHIST_D02032026.TXT");
        var file2 = Path.Combine(_testPath, "COTAHIST_D01032026.TXT");
        var file3 = Path.Combine(_testPath, "INVALID.TXT");

        File.WriteAllText(file1, "test");
        File.WriteAllText(file2, "test");
        File.WriteAllText(file3, "test");

        var explorer = new FileExplorer(_configMock.Object);

        // Act
        var result = explorer.GetPendingFiles().ToList();

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Equal("COTAHIST_D01032026.TXT", result[0].Name);
        Assert.Equal("COTAHIST_D02032026.TXT", result[1].Name);
    }

    [Theory]
    [InlineData("COTAHIST_D01032026.TXT", 2026, 3, 1)]
    [InlineData("COTAHIST_D25122025.TXT", 2025, 12, 25)]
    public void GetDateFromFileName_WithValidName_ShouldReturnCorrectDate(string fileName, int year, int month, int day)
    {
        // Arrange
        var explorer = new FileExplorer(_configMock.Object);

        // Act
        var result = explorer.GetDateFromFileName(fileName);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(new DateTime(year, month, day), result.Value);
    }

    [Fact]
    public void GetDateFromFileName_WithInvalidName_ShouldReturnNull()
    {
        // Arrange
        var explorer = new FileExplorer(_configMock.Object);

        // Act
        var result = explorer.GetDateFromFileName("INVALID.TXT");

        // Assert
        Assert.Null(result);
    }

    public void Dispose()
    {
        if (Directory.Exists(_testPath))
            Directory.Delete(_testPath, true);
    }
}
