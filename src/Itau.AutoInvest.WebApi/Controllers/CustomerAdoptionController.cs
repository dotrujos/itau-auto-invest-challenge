using Itau.AutoInvest.Application.UseCases.CustomerAdoption;
using Itau.AutoInvest.Application.UseCases.CustomerAdoption.IO;
using Microsoft.AspNetCore.Mvc;

namespace Itau.AutoInvest.WebApi.Controllers;

[ApiController]
[Route("api/clientes")]
public class CustomerAdoptionController : ControllerBase
{
    private readonly CustomerAdoption _customerAdoption;

    public CustomerAdoptionController(CustomerAdoption customerAdoption)
    {
        _customerAdoption = customerAdoption;
    }

    [HttpPost("adesao")]
    public async Task<IActionResult> Adopt([FromBody] CustomerAdoptionInput input, CancellationToken ct)
    {
        var output = await _customerAdoption.ExecuteAsync(input, ct);

        return StatusCode(201, output);
    }
}
