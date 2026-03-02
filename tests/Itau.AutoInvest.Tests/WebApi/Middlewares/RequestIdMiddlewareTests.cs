using Itau.AutoInvest.WebApi.Middlewares;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Itau.AutoInvest.Tests.WebApi.Middlewares;

public class RequestIdMiddlewareTests
{
    private readonly Mock<ILogger<RequestIdMiddleware>> _loggerMock;
    private readonly DefaultHttpContext _context;

    public RequestIdMiddlewareTests()
    {
        _loggerMock = new Mock<ILogger<RequestIdMiddleware>>();
        _context = new DefaultHttpContext();
        
        var serviceProviderMock = new Mock<IServiceProvider>();
        serviceProviderMock
            .Setup(x => x.GetService(typeof(ILogger<RequestIdMiddleware>)))
            .Returns(_loggerMock.Object);
            
        _context.RequestServices = serviceProviderMock.Object;
    }

    [Fact]
    public async Task InvokeAsync_ShouldAddRequestIdHeaderAndCallNext()
    {
        // Arrange
        var nextCalled = false;
        RequestDelegate next = (ctx) =>
        {
            nextCalled = true;
            return Task.CompletedTask;
        };

        var sut = new RequestIdMiddleware(next);

        // Act
        await sut.InvokeAsync(_context);

        // Assert
        Assert.True(nextCalled);
        Assert.True(_context.Response.Headers.ContainsKey("X-Request-Id"));
        Assert.False(string.IsNullOrEmpty(_context.Response.Headers["X-Request-Id"]));
    }
}
