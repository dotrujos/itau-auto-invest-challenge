using Itau.AutoInvest.Application.UseCases.UpdateRecommendationBasket;
using Itau.AutoInvest.Application.UseCases.UpdateRecommendationBasket.IO;
using Itau.AutoInvest.WebApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace Itau.AutoInvest.WebApi.Controllers;

[ApiController]
[Route("api/admin/cesta")]
[Tags("Administrativo")]
public class UpdateRecommendationBasketController : ControllerBase
{
    private readonly UpdateRecommendationBasket _updateRecommendationBasket;

    public UpdateRecommendationBasketController(UpdateRecommendationBasket updateRecommendationBasket)
    {
        _updateRecommendationBasket = updateRecommendationBasket;
    }

    [HttpPost]
    [EndpointSummary("Atualizar cesta")]
    [EndpointDescription("Cadastra ou altera a Cesta Top Five de recomendação.")]
    [ProducesResponseType(typeof(UpdateRecommendationBasketOutput), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Create([FromBody] UpdateRecommendationBasketInput input, CancellationToken ct)
    {
        var output = await _updateRecommendationBasket.ExecuteAsync(input, ct);
        return CreatedAtAction(null, new { id = output.BasketId }, output);
    }
}
