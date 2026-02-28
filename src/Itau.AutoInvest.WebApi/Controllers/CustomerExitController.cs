using Itau.AutoInvest.Application.UseCases.CustomerExit;
using Itau.AutoInvest.Application.UseCases.CustomerExit.IO;
using Microsoft.AspNetCore.Mvc;

namespace Itau.AutoInvest.WebApi.Controllers;

[ApiController]
[Route("api/clientes")]
public class CustomerExitController : ControllerBase
{
    private readonly CustomerExit _customerExit;

    public CustomerExitController(CustomerExit customerExit)
    {
        _customerExit = customerExit;
    }

    [HttpPost("{clienteId}/saida")]
    public async Task<IActionResult> Exit([FromRoute] long clienteId, CancellationToken ct)
    {
        var input = new CustomerExitInput(clienteId);
        var output = await _customerExit.ExecuteAsync(input, ct);
        
        return Ok(output);
    }
}
