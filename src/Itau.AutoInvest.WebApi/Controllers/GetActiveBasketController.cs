using Itau.AutoInvest.Application.UseCases.GetActiveBasket;
using Itau.AutoInvest.Application.UseCases.GetActiveBasket.IO;
using Microsoft.AspNetCore.Mvc;

namespace Itau.AutoInvest.WebApi.Controllers;

[ApiController]
[Route("api/admin/cesta")]
[Tags("Administrativo")]
public class GetActiveBasketController : ControllerBase
{
    private readonly GetActiveBasket _getActiveBasket;

    public GetActiveBasketController(GetActiveBasket getActiveBasket)
    {
        _getActiveBasket = getActiveBasket;
    }

    [HttpGet("atual")]
    [EndpointSummary("Consultar cesta ativa")]
    [EndpointDescription("Consulta a cesta de recomendação atual (ativa).")]
    [ProducesResponseType(typeof(GetActiveBasketOutput), 200)]
    [ProducesResponseType(typeof(ProblemDetails), 404)]
    public async Task<IActionResult> GetActive(CancellationToken ct)
    {
        var output = await _getActiveBasket.ExecuteAsync(ct);
        
        return Ok(output);
    }
}
