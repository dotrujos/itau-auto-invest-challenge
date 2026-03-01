using Itau.AutoInvest.Application.UseCases.UpdateMonthlyInvestment;
using Itau.AutoInvest.Application.UseCases.UpdateMonthlyInvestment.IO;
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
    [ProducesResponseType(typeof(UpdateMonthlyInvestmentOutput), 200)]
    [ProducesResponseType(typeof(ProblemDetails), 400)]
    [ProducesResponseType(typeof(ProblemDetails), 404)]
    public async Task<IActionResult> UpdateValue([FromRoute] long clienteId, [FromBody] UpdateMonthlyInvestmentInput input, CancellationToken ct)
    {
        // Garante que o ID da rota seja o mesmo do input (que está ignorado no body)
        var updatedInput = input with { ClientId = clienteId };
        
        var output = await _updateMonthlyInvestment.ExecuteAsync(updatedInput, ct);
        
        return Ok(output);
    }
}
