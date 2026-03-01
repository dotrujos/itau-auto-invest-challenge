using Itau.AutoInvest.Application.UseCases.GetClientPortfolio;
using Itau.AutoInvest.Application.UseCases.GetClientPortfolio.IO;
using Itau.AutoInvest.Application.UseCases.GetDetailedProfitability;
using Itau.AutoInvest.Application.UseCases.GetDetailedProfitability.IO;
using Microsoft.AspNetCore.Mvc;

namespace Itau.AutoInvest.WebApi.Controllers;

[ApiController]
[Route("api/clientes")]
[Tags("Clientes")]
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
    [EndpointSummary("Consultar carteira")]
    [EndpointDescription("Consulta a carteira de custódia do cliente, incluindo ativos e resumo de PL.")]
    [ProducesResponseType(typeof(GetClientPortfolioOutput), 200)]
    [ProducesResponseType(typeof(ProblemDetails), 404)]
    public async Task<IActionResult> GetPortfolio([FromRoute] long clienteId, CancellationToken ct)
    {
        var input = new GetClientPortfolioInput(clienteId);
        var output = await _getClientPortfolio.ExecuteAsync(input, ct);
        
        return Ok(output);
    }

    [HttpGet("{clienteId}/rentabilidade")]
    [EndpointSummary("Consultar rentabilidade")]
    [EndpointDescription("Consulta a rentabilidade detalhada do cliente, histórico de aportes e evolução da carteira.")]
    [ProducesResponseType(typeof(GetDetailedProfitabilityOutput), 200)]
    [ProducesResponseType(typeof(ProblemDetails), 404)]
    public async Task<IActionResult> GetProfitability([FromRoute] long clienteId, CancellationToken ct)
    {
        var input = new GetDetailedProfitabilityInput(clienteId);
        var output = await _getDetailedProfitability.ExecuteAsync(input, ct);
        
        return Ok(output);
    }
}
