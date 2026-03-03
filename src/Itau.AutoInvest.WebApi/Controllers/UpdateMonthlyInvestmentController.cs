using Itau.AutoInvest.Application.UseCases.UpdateMonthlyInvestment;
using Itau.AutoInvest.Application.UseCases.UpdateMonthlyInvestment.IO;
using Itau.AutoInvest.WebApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace Itau.AutoInvest.WebApi.Controllers;

[ApiController]
[Route("api/clientes")]
[Tags("Clientes")]
public class UpdateMonthlyInvestmentController : ControllerBase
{
    private readonly UpdateMonthlyInvestment _updateMonthlyInvestment;

    public UpdateMonthlyInvestmentController(UpdateMonthlyInvestment updateMonthlyInvestment)
    {
        _updateMonthlyInvestment = updateMonthlyInvestment;
    }

    [HttpPut("{clienteId}/valor-mensal")]
    [EndpointSummary("Atualizar aporte mensal")]
    [EndpointDescription("Altera o valor do aporte mensal do cliente.")]
    [ProducesResponseType(typeof(UpdateMonthlyInvestmentOutput), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateValue([FromRoute] long clienteId, [FromBody] UpdateMonthlyInvestmentInput input, CancellationToken ct)
    {
        var updatedInput = input with { ClientId = clienteId };
        
        var output = await _updateMonthlyInvestment.ExecuteAsync(updatedInput, ct);
        
        return Ok(output);
    }
}
