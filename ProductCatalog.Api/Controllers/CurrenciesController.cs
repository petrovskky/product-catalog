using MediatR;
using Microsoft.AspNetCore.Mvc;
using ProductCatalog.Application.UseCases.Currencies.Queries.GetPrice;

namespace ProductCatalog.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CurrenciesController(IMediator mediator) : ControllerBase
{
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetRate(int id, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetRateQuery(id), 
            cancellationToken);
    
        if (result.IsFailure)
            return BadRequest(new { result.Error });
    
        return Ok(new { rate = result.Value });
    }
}