using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using ProductCatalog.Application.Common.Interfaces;

namespace ProductCatalog.Infrastructure.Services;

public class UserContext : IUserContext
{
    public UserContext(IHttpContextAccessor httpContextAccessor)
    {
        var user = httpContextAccessor.HttpContext?.User;

        IsAuthenticated = user?.Identity?.IsAuthenticated == true;

        if (IsAuthenticated)
        {
            var idClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            Id = Guid.TryParse(idClaim, out var id) ? id : null;

            Email = user.FindFirst(ClaimTypes.Email)?.Value;
            Name = user.FindFirst(ClaimTypes.Name)?.Value;

            Role = user.Claims
                .Where(c => c.Type == ClaimTypes.Role)
                .Select(c => c.Value)
                .FirstOrDefault() ?? null;
        }
        else
        {
            Role = "Guest";
        }
    }

    public Guid? Id { get; }
    public string? Name { get; }
    public string? Email { get; }
    public string? Role { get; }
    public bool IsAuthenticated { get; }
}