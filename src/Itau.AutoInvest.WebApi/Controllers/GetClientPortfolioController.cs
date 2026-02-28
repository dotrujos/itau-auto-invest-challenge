using Itau.AutoInvest.Application.UseCases.GetClientPortfolio;
using Itau.AutoInvest.Application.UseCases.GetClientPortfolio.IO;
using Itau.AutoInvest.Application.UseCases.GetDetailedProfitability;
using Itau.AutoInvest.Application.UseCases.GetDetailedProfitability.IO;
using Microsoft.AspNetCore.Mvc;

namespace Itau.AutoInvest.WebApi.Controllers;

[ApiController]
[Route("api/clientes")]
public class GetClientPortfolioController : ControllerBase
{
    private readonly GetClientPortfolio _getClientPortfolio;
    private readonly GetDetailedProfitability _getDetailedProfitability;

    public GetClientPortfolioController(GetClientPortfolio getClientPortfolio, GetDetailedProfitability getDetailedProfitability)
    {
        _getClientPortfolio = getClientPortfolio;
        _getDetailedProfitability = getDetailedProfitability;
    }

    [HttpGet("{clienteId}/carteira")]
    public async Task<IActionResult> GetPortfolio([FromRoute] long clienteId, CancellationToken ct)
    {
        var input = new GetClientPortfolioInput(clienteId);
        var output = await _getClientPortfolio.ExecuteAsync(input, ct);
        
        return Ok(output);
    }

    [HttpGet("{clienteId}/rentabilidade")]
    public async Task<IActionResult> GetProfitability([FromRoute] long clienteId, CancellationToken ct)
    {
        var input = new GetDetailedProfitabilityInput(clienteId);
        var output = await _getDetailedProfitability.ExecuteAsync(input, ct);
        
        return Ok(output);
    }
}
