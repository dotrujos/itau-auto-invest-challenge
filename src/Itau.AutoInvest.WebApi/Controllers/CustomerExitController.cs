using Itau.AutoInvest.Application.UseCases.CustomerExit;
using Itau.AutoInvest.Application.UseCases.CustomerExit.IO;
using Itau.AutoInvest.WebApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace Itau.AutoInvest.WebApi.Controllers;

[ApiController]
[Route("api/clientes")]
[Tags("Clientes")]
public class CustomerExitController : ControllerBase
{
    private readonly CustomerExit _customerExit;

    public CustomerExitController(CustomerExit customerExit)
    {
        _customerExit = customerExit;
    }

    [HttpPost("{clienteId}/saida")]
    [EndpointSummary("Saída do cliente")]
    [EndpointDescription("Encerra a adesão do cliente ao produto.")]
    [ProducesResponseType(typeof(CustomerExitOutput), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Exit([FromRoute] long clienteId, CancellationToken ct)
    {
        var input = new CustomerExitInput(clienteId);
        var output = await _customerExit.ExecuteAsync(input, ct);
        
        return Ok(output);
    }
}
