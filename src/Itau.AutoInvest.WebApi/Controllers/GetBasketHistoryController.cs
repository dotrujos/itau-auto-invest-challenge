using Itau.AutoInvest.Application.UseCases.GetBasketHistory;
using Itau.AutoInvest.Application.UseCases.GetBasketHistory.IO;
using Microsoft.AspNetCore.Mvc;

namespace Itau.AutoInvest.WebApi.Controllers;

[ApiController]
[Route("api/admin/cesta")]
public class GetBasketHistoryController : ControllerBase
{
    private readonly GetBasketHistory _getBasketHistory;

    public GetBasketHistoryController(GetBasketHistory getBasketHistory)
    {
        _getBasketHistory = getBasketHistory;
    }

    [HttpGet("historico")]
    public async Task<IActionResult> GetHistory(CancellationToken ct)
    {
        var input = new GetBasketHistoryInput();
        var output = await _getBasketHistory.ExecuteAsync(input, ct);
        
        return Ok(output);
    }
}
