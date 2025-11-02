using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductCatalog.Api.Dtos.Products;
using ProductCatalog.Api.Extensions;
using ProductCatalog.Application.Common;
using ProductCatalog.Application.Filters;
using ProductCatalog.Application.UseCases.Products.Commands.Create;
using ProductCatalog.Application.UseCases.Products.Commands.Delete;
using ProductCatalog.Application.UseCases.Products.Commands.Update;
using ProductCatalog.Application.UseCases.Products.Queries.Get;
using ProductCatalog.Application.UseCases.Products.Queries.List;

namespace ProductCatalog.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProductsController(IMediator mediator) : ControllerBase
{
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Get(Guid id, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetProductQuery(id, User.GetRole()), 
            cancellationToken);
    
        if (result.IsFailure)
            return BadRequest(new { error = result.Error });
    
        return Ok(result.Value);
    }
    
    [HttpGet]
    public async Task<IActionResult> List(
        [FromQuery] ProductFilter productFilter,
        [FromQuery] SortParams sortParams,
        [FromQuery] PageParams pageParams,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(
            new ListProductQuery(User.GetRole(), productFilter, sortParams, pageParams), 
            cancellationToken);
    
        return Ok(result.Value);
    }
    
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Create([FromBody] ProductRequest request, 
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(
            new CreateProductCommand(User.GetRole(), request.Name, request.CategoryId, request.Description, 
                request.Price, request.Note, request.SpecialNote),
            cancellationToken);
        
        if (result.IsFailure)
            return BadRequest(new { error = result.Error });
    
        return CreatedAtAction(
            nameof(Get), new { id = result.Value }, new { id = result.Value });
    }
    
    [HttpPut("{id:guid}")]
    [Authorize]
    public async Task<IActionResult> Update(Guid id, [FromBody] ProductRequest request, 
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(
            new UpdateProductCommand(id, request.Name, request.CategoryId, request.Description, 
                request.Price, request.Note, request.SpecialNote), 
            cancellationToken);
        
        if (result.IsFailure)
            return BadRequest(new { error = result.Error });
    
        return NoContent();
    }
    
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "ProUser,Admin")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new DeleteProductCommand(id), cancellationToken);
    
        if (result.IsFailure)
            return BadRequest(new { error = result.Error });
    
        return NoContent();
    }
}

