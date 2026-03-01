using Itau.AutoInvest.Application.UseCases.UpdateRecommendationBasket;
using Itau.AutoInvest.Application.UseCases.UpdateRecommendationBasket.IO;
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
    [ProducesResponseType(typeof(UpdateRecommendationBasketOutput), 201)]
    [ProducesResponseType(typeof(ProblemDetails), 400)]
    public async Task<IActionResult> Create([FromBody] UpdateRecommendationBasketInput input, CancellationToken ct)
    {
        var output = await _updateRecommendationBasket.ExecuteAsync(input, ct);
        return CreatedAtAction(null, new { id = output.BasketId }, output);
    }
}
