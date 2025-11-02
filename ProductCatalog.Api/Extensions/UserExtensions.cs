using System.Security.Claims;

namespace ProductCatalog.Api.Extensions;

public static class UserExtensions
{
    public static string GetRole(this ClaimsPrincipal user)
    {
        return user.FindFirst(ClaimTypes.Role)?.Value ?? string.Empty;
    }
    public static string GetName(this ClaimsPrincipal user)
    {
        return user.FindFirst(ClaimTypes.Name)?.Value ?? string.Empty;
    }
    public static string GetEmail(this ClaimsPrincipal user)
    {
        return user.FindFirst(ClaimTypes.Email)?.Value ?? string.Empty;
    }
    public static Guid GetId(this ClaimsPrincipal user)
    {
        var idClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return string.IsNullOrEmpty(idClaim) ? Guid.Empty : Guid.Parse(idClaim);
    }
}