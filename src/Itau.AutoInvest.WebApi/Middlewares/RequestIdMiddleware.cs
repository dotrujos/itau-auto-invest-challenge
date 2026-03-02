namespace Itau.AutoInvest.WebApi.Middlewares;

public class RequestIdMiddleware
{
    private readonly RequestDelegate _next;
    private const string RequestIdHeaderName = "X-Request-Id";

    public RequestIdMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var requestId = Guid.CreateVersion7().ToString();
        
        if (!context.Response.Headers.ContainsKey(RequestIdHeaderName))
        {
            context.Response.Headers.Append(RequestIdHeaderName, requestId);
        }
        
        using var scope = context.RequestServices.GetRequiredService<ILogger<RequestIdMiddleware>>().BeginScope(new Dictionary<string, object>
        {
            ["RequestId"] = requestId
        });
        await _next(context);
    }
}
