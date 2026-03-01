using Itau.AutoInvest.Application.UseCases.CustomerExit;
using Itau.AutoInvest.Application.UseCases.CustomerExit.IO;
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
    [ProducesResponseType(typeof(CustomerExitOutput), 200)]
    [ProducesResponseType(typeof(ProblemDetails), 404)]
    public async Task<IActionResult> Exit([FromRoute] long clienteId, CancellationToken ct)
    {
        var input = new CustomerExitInput(clienteId);
        var output = await _customerExit.ExecuteAsync(input, ct);
        
        return Ok(output);
    }
}
