using Itau.AutoInvest.Application.UseCases.GetActiveBasket;
using Itau.AutoInvest.Application.UseCases.GetActiveBasket.IO;
using Microsoft.AspNetCore.Mvc;

namespace Itau.AutoInvest.WebApi.Controllers;

[ApiController]
[Route("api/admin/cesta")]
public class GetActiveBasketController : ControllerBase
{
    private readonly GetActiveBasket _getActiveBasket;

    public GetActiveBasketController(GetActiveBasket getActiveBasket)
    {
        _getActiveBasket = getActiveBasket;
    }

    [HttpGet("atual")]
    public async Task<IActionResult> GetActive(CancellationToken ct)
    {
        var input = new GetActiveBasketInput();
        var output = await _getActiveBasket.ExecuteAsync(input, ct);
        
        return Ok(output);
    }
}
