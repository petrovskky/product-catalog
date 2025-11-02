using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductCatalog.Api.Dtos.Users;
using ProductCatalog.Api.Extensions;
using ProductCatalog.Application.UseCases.Users.Commands.Block;
using ProductCatalog.Application.UseCases.Users.Commands.ChangePassword;
using ProductCatalog.Application.UseCases.Users.Commands.Create;
using ProductCatalog.Application.UseCases.Users.Commands.Delete;
using ProductCatalog.Application.UseCases.Users.Queries;
using ProductCatalog.Application.UseCases.Users.Queries.Get;
using ProductCatalog.Application.UseCases.Users.Queries.List;

namespace ProductCatalog.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = "Admin")]
public class UsersController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> List(CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new ListUserQuery(), cancellationToken);
        
        return Ok(result.Value);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Get(Guid id, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetUserByIdQuery(id), cancellationToken);
        
        if (result.IsFailure)
            return BadRequest(new { error = result.Error });
        
        return Ok(result.Value);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateUserRequest request,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(
            new CreateUserCommand(request.Email, request.Name, request.Role, request.Password),
            cancellationToken);
        
        if (result.IsFailure)
            return BadRequest(new { error = result.Error });
    
        return CreatedAtAction(
            nameof(Get), new { id = result.Value }, new { id = result.Value });
    }
    
    [HttpPut("{id:guid}/block")]
    public async Task<IActionResult> Block(Guid id, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new BlockUserCommand(id), cancellationToken);
        
        if (result.IsFailure)
            return BadRequest(new { error = result.Error });
        
        return NoContent();
    }
    
    [HttpPut("{id}/password")]
    public async Task<IActionResult> ChangeUserPassword(Guid id,
        [FromBody] ChangePasswordRequest request, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(
            new ChangePasswordCommand(id, request.NewPassword),
            cancellationToken);
        
        if (result.IsFailure)
            return BadRequest(new { error = result.Error });
        
        return NoContent();
    }
    
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new DeleteUserCommand(id), cancellationToken);
    
        if (result.IsFailure)
            return BadRequest(new { error = result.Error });
    
        return NoContent();
    }
}
