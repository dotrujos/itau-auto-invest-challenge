using Itau.AutoInvest.Application.UseCases.GetClientPortfolio;
using Itau.AutoInvest.Application.UseCases.GetClientPortfolio.IO;
using Microsoft.AspNetCore.Mvc;

namespace Itau.AutoInvest.WebApi.Controllers;

[ApiController]
[Route("api/clientes")]
public class GetClientPortfolioController : ControllerBase
{
    private readonly GetClientPortfolio _getClientPortfolio;

    public GetClientPortfolioController(GetClientPortfolio getClientPortfolio)
    {
        _getClientPortfolio = getClientPortfolio;
    }

    [HttpGet("{clienteId}/carteira")]
    public async Task<IActionResult> GetPortfolio([FromRoute] long clienteId, CancellationToken ct)
    {
        var input = new GetClientPortfolioInput(clienteId);
        var output = await _getClientPortfolio.ExecuteAsync(input, ct);
        
        return Ok(output);
    }
}
