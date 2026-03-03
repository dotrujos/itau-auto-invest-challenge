using Itau.AutoInvest.Application.UseCases.ExecuteManualPurchase;
using Itau.AutoInvest.Application.UseCases.ExecuteManualPurchase.IO;
using Itau.AutoInvest.WebApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace Itau.AutoInvest.WebApi.Controllers;

[ApiController]
[Route("api/motor")]
[Tags("Motor de Compra")]
public class ExecuteManualPurchaseController : ControllerBase
{
    private readonly ExecuteManualPurchase _executeManualPurchase;

    public ExecuteManualPurchaseController(ExecuteManualPurchase executeManualPurchase)
    {
        _executeManualPurchase = executeManualPurchase;
    }

    [HttpPost("executar-compra")]
    [EndpointSummary("Executar compra programada")]
    [EndpointDescription("Dispara o motor de compra programada para consolidar aportes e distribuir ativos entre clientes.")]
    [ProducesResponseType(typeof(ExecuteManualPurchaseOutput), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ExecuteManual([FromBody] ExecuteManualPurchaseInput input, CancellationToken ct)
    {
        var output = await _executeManualPurchase.ExecuteAsync(input, ct);
        
        return Ok(output);
    }
}
