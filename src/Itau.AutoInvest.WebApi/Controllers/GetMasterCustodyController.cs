using Itau.AutoInvest.Application.UseCases.GetMasterCustody;
using Itau.AutoInvest.Application.UseCases.GetMasterCustody.IO;
using Microsoft.AspNetCore.Mvc;

namespace Itau.AutoInvest.WebApi.Controllers;

[ApiController]
[Route("api/admin/conta-master")]
[Tags("Administrativo")]
public class GetMasterCustodyController : ControllerBase
{
    private readonly GetMasterCustody _getMasterCustody;

    public GetMasterCustodyController(GetMasterCustody getMasterCustody)
    {
        _getMasterCustody = getMasterCustody;
    }

    [HttpGet("custodia")]
    [EndpointSummary("Consultar custódia master")]
    [EndpointDescription("Consulta os resíduos de ativos que ficaram na conta master após a distribuição fracionária.")]
    [ProducesResponseType(typeof(GetMasterCustodyOutput), 200)]
    [ProducesResponseType(typeof(ProblemDetails), 404)]
    [ProducesResponseType(typeof(ProblemDetails), 500)]
    public async Task<IActionResult> GetMasterCustody(CancellationToken ct)
    {
        var output = await _getMasterCustody.ExecuteAsync(ct);
        
        return Ok(output);
    }
}
