using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductCatalog.Api.Dtos.Categories;
using ProductCatalog.Application.UseCases.Categories.Commands.Create;
using ProductCatalog.Application.UseCases.Categories.Commands.Delete;
using ProductCatalog.Application.UseCases.Categories.Commands.Update;
using ProductCatalog.Application.UseCases.Categories.Queries.Get;
using ProductCatalog.Application.UseCases.Categories.Queries.List;

namespace ProductCatalog.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CategoriesController(IMediator mediator) : ControllerBase
{
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Get(Guid id, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetCategoryQuery(id), cancellationToken);

        if (result.IsFailure)
            return BadRequest(new { result.Error });

        return Ok(result.Value);
    }
    
    [HttpGet]
    public async Task<IActionResult> List(CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new ListCategoryQuery(), cancellationToken);
    
        return Ok(result.Value);
    }

    [HttpPost]
    [Authorize(Roles = "ProUser,Admin")]
    public async Task<IActionResult> Create([FromBody] CategoryRequest request, 
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new CreateCategoryCommand(request.Name),
            cancellationToken);

        return CreatedAtAction(
            nameof(Get), new { id = result.Value }, new { id = result.Value });
    }
    
    [HttpPut("{id:guid}")]
    [Authorize(Roles = "ProUser,Admin")]
    public async Task<IActionResult> Update(Guid id, [FromBody] CategoryRequest request, 
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new UpdateCategoryCommand(id, request.Name),
            cancellationToken);
        
        if (result.IsFailure)
            return BadRequest(new { error = result.Error });

        return NoContent();
    }
    
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "ProUser,Admin")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new DeleteCategoryCommand(id), cancellationToken);

        if (result.IsFailure)
            return BadRequest(new { error = result.Error });

        return NoContent();
    }
}
