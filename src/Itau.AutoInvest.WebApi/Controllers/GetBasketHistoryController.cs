using Itau.AutoInvest.Application.UseCases.GetBasketHistory;
using Itau.AutoInvest.Application.UseCases.GetBasketHistory.IO;
using Microsoft.AspNetCore.Mvc;

namespace Itau.AutoInvest.WebApi.Controllers;

[ApiController]
[Route("api/admin/cesta")]
[Tags("Administrativo")]
public class GetBasketHistoryController : ControllerBase
{
    private readonly GetBasketHistory _getBasketHistory;

    public GetBasketHistoryController(GetBasketHistory getBasketHistory)
    {
        _getBasketHistory = getBasketHistory;
    }

    [HttpGet("historico")]
    [EndpointSummary("Histórico de cestas")]
    [EndpointDescription("Consulta o histórico de todas as cestas de recomendação (ativas e inativas).")]
    [ProducesResponseType(typeof(GetBasketHistoryOutput), 200)]
    public async Task<IActionResult> GetHistory(CancellationToken ct)
    {
        var input = new GetBasketHistoryInput();
        var output = await _getBasketHistory.ExecuteAsync(input, ct);
        
        return Ok(output);
    }
}
