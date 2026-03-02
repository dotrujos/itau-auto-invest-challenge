using System.Net;
using System.Text.Json;
using Itau.AutoInvest.Domain.Exceptions;
using Itau.AutoInvest.WebApi.Middlewares;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Itau.AutoInvest.Tests.WebApi.Middlewares;

public class GlobalExceptionMiddlewareTests
{
    private readonly Mock<ILogger<GlobalExceptionMiddleware>> _loggerMock;
    private readonly DefaultHttpContext _context;

    public GlobalExceptionMiddlewareTests()
    {
        _loggerMock = new Mock<ILogger<GlobalExceptionMiddleware>>();
        _context = new DefaultHttpContext();
        _context.Response.Body = new MemoryStream();
    }

    [Fact]
    public async Task InvokeAsync_WhenNoExceptionOccurs_ShouldCallNext()
    {
        // Arrange
        var nextMock = new Mock<RequestDelegate>();
        var sut = new GlobalExceptionMiddleware(nextMock.Object, _loggerMock.Object);

        // Act
        await sut.InvokeAsync(_context);

        // Assert
        nextMock.Verify(next => next(It.IsAny<HttpContext>()), Times.Once);
    }

    [Theory]
    [InlineData(typeof(DuplicateCpfException), (int)HttpStatusCode.BadRequest, "CLIENTE_CPF_DUPLICADO")]
    [InlineData(typeof(ClientNotFoundException), (int)HttpStatusCode.NotFound, "CLIENTE_NAO_ENCONTRADO")]
    [InlineData(typeof(PurchaseAlreadyExecutedException), (int)HttpStatusCode.Conflict, "COMPRA_JA_EXECUTADA")]
    [InlineData(typeof(KafkaUnavailableException), (int)HttpStatusCode.InternalServerError, "KAFKA_INDISPONIVEL")]
    public async Task InvokeAsync_WhenDomainExceptionOccurs_ShouldHandleCorrectly(Type exceptionType, int expectedStatusCode, string expectedCode)
    {
        // Arrange
        BaseDomainException exception;
        if (exceptionType == typeof(PurchaseAlreadyExecutedException))
            exception = new PurchaseAlreadyExecutedException(DateTime.Now);
        else if (exceptionType == typeof(ClientNotFoundException))
            exception = new ClientNotFoundException();
        else if (exceptionType == typeof(DuplicateCpfException))
            exception = new DuplicateCpfException();
        else
            exception = (BaseDomainException)Activator.CreateInstance(exceptionType)!;

        var nextMock = new Mock<RequestDelegate>();
        nextMock.Setup(next => next(It.IsAny<HttpContext>())).ThrowsAsync(exception);
        
        var sut = new GlobalExceptionMiddleware(nextMock.Object, _loggerMock.Object);

        // Act
        await sut.InvokeAsync(_context);

        // Assert
        Assert.Equal(expectedStatusCode, _context.Response.StatusCode);
        Assert.Equal("application/json", _context.Response.ContentType);

        _context.Response.Body.Seek(0, SeekOrigin.Begin);
        var body = await new StreamReader(_context.Response.Body).ReadToEndAsync();
        var response = JsonSerializer.Deserialize<Dictionary<string, string>>(body);

        Assert.NotNull(response);
        Assert.Equal(exception.Message, response["erro"]);
        Assert.Equal(expectedCode, response["codigo"]);
    }

    [Fact]
    public async Task InvokeAsync_WhenUnexpectedExceptionOccurs_ShouldReturnInternalServerError()
    {
        // Arrange
        var nextMock = new Mock<RequestDelegate>();
        nextMock.Setup(next => next(It.IsAny<HttpContext>())).ThrowsAsync(new Exception("Unexpected error"));
        
        var sut = new GlobalExceptionMiddleware(nextMock.Object, _loggerMock.Object);

        // Act
        await sut.InvokeAsync(_context);

        // Assert
        Assert.Equal((int)HttpStatusCode.InternalServerError, _context.Response.StatusCode);
        Assert.Equal("application/json", _context.Response.ContentType);

        _context.Response.Body.Seek(0, SeekOrigin.Begin);
        var body = await new StreamReader(_context.Response.Body).ReadToEndAsync();
        var response = JsonSerializer.Deserialize<Dictionary<string, string>>(body);

        Assert.NotNull(response);
        Assert.Equal("Ocorreu um erro interno inesperado no servidor.", response["erro"]);
        Assert.Equal("ERRO_INTERNO", response["codigo"]);
    }
}
