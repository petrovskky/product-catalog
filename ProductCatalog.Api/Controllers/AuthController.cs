using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using ProductCatalog.Api.Configuration;
using ProductCatalog.Api.Dtos.Users;
using ProductCatalog.Api.Extensions;
using ProductCatalog.Application.UseCases.Auth.Login;
using ProductCatalog.Application.UseCases.Auth.Register;

namespace ProductCatalog.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController(IMediator mediator, IOptions<CookieSettings> cookieSettings) : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterUserRequest request, 
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(
            new RegisterUserCommand(request.Email, request.Name,  request.Password),
            cancellationToken);

        if (result.IsFailure)
            return Conflict(new { error = result.Error });

        return NoContent();
    }
    
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginUserRequest request,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new LoginUserCommand(request.Email, request.Password),
            cancellationToken);
        
        if (result.IsFailure)
            return result.Error!.Contains("заблокирован", StringComparison.OrdinalIgnoreCase) 
                ? StatusCode(StatusCodes.Status423Locked, new { error = result.Error }) 
                : Unauthorized(new { error = result.Error });
        
        HttpContext.Response.Cookies.Append(cookieSettings.Value.Name, result.Value, new CookieOptions
        {
            HttpOnly = cookieSettings.Value.HttpOnly,
            Secure = cookieSettings.Value.Secure,
            SameSite = cookieSettings.Value.GetSameSiteMode(),
            Path = cookieSettings.Value.Path,
            Expires = DateTimeOffset.UtcNow.AddHours(cookieSettings.Value.ExpiresHours)
        });
        
        return Ok();
    }
    
    [HttpPost("logout")]
    public IActionResult Logout()
    {
        HttpContext.Response.Cookies.Delete(cookieSettings.Value.Name, new CookieOptions
        {
            Path = cookieSettings.Value.Path
        });

        return Ok();
    }
    
    [HttpGet("me")]
    [Authorize]
    public IActionResult GetCurrentUser()
    {
        var id = User.GetId();
        var email = User.GetEmail();
        var name = User.GetName();
        var role = User.GetRole();

        return Ok(new { id, email, name, role });
    }
}

