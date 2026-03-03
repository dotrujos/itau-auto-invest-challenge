using Itau.AutoInvest.Application.UseCases.CustomerAdoption;
using Itau.AutoInvest.Application.UseCases.CustomerAdoption.IO;
using Itau.AutoInvest.WebApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace Itau.AutoInvest.WebApi.Controllers;

[ApiController]
[Route("api/clientes")]
[Tags("Clientes")]
public class CustomerAdoptionController : ControllerBase
{
    private readonly CustomerAdoption _customerAdoption;

    public CustomerAdoptionController(CustomerAdoption customerAdoption)
    {
        _customerAdoption = customerAdoption;
    }

    [HttpPost("adesao")]
    [EndpointSummary("Adesão do cliente")]
    [EndpointDescription("Realiza a adesão do cliente ao produto de compra programada.")]
    [ProducesResponseType(typeof(CustomerAdoptionOutput), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Adopt([FromBody] CustomerAdoptionInput input, CancellationToken ct)
    {
        var output = await _customerAdoption.ExecuteAsync(input, ct);

        return StatusCode(201, output);
    }
}
