using System.Net;
using System.Text.Json;
using Itau.AutoInvest.Domain.Exceptions;
using Itau.AutoInvest.WebApi.Models;

namespace Itau.AutoInvest.WebApi.Middlewares;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (BaseDomainException ex)
        {
            _logger.LogWarning(ex, "Domain Exception: {Code} - {Message}", ex.Code, ex.Message);
            await HandleDomainExceptionAsync(context, ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected Exception occurred.");
            await HandleUnexpectedExceptionAsync(context);
        }
    }

    private static Task HandleDomainExceptionAsync(HttpContext context, BaseDomainException exception)
    {
        context.Response.ContentType = "application/json";
        
        // Mapeia o Status Code com base na exceção específica (conforme o contrato do desafio)
        context.Response.StatusCode = exception switch
        {
            DuplicateCpfException => (int)HttpStatusCode.BadRequest,
            InvalidMonthlyValueException => (int)HttpStatusCode.BadRequest,
            InvalidBasketPercentageException => (int)HttpStatusCode.BadRequest,
            InvalidBasketQuantityException => (int)HttpStatusCode.BadRequest,
            ClientAlreadyInactiveException => (int)HttpStatusCode.BadRequest,
            
            ClientNotFoundException => (int)HttpStatusCode.NotFound,
            BasketNotFoundException => (int)HttpStatusCode.NotFound,
            QuoteNotFoundException => (int)HttpStatusCode.NotFound,
            MasterAccountNotFoundException => (int)HttpStatusCode.NotFound,
            EntityNotFoundException => (int)HttpStatusCode.NotFound,
            
            PurchaseAlreadyExecutedException => (int)HttpStatusCode.Conflict,
            
            KafkaUnavailableException => (int)HttpStatusCode.InternalServerError,
            
            _ => (int)HttpStatusCode.BadRequest
        };

        var response = new ErrorResponse
        {
            Erro = exception.Message,
            Codigo = exception.Code
        };

        return context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }

    private static Task HandleUnexpectedExceptionAsync(HttpContext context)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

        var response = new ErrorResponse
        {
            Erro = "Ocorreu um erro interno inesperado no servidor.",
            Codigo = "ERRO_INTERNO"
        };

        return context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }
}
