namespace ProductCatalog.Application.Common.Services;

public static class RolePrivileges
{
    private static readonly string[] PrivilegedRoles = ["ProUser", "Admin"];
    
    public static bool HasPrivilegedAccess(string userRole)
    {
        return PrivilegedRoles.Any(role => PrivilegedRoles.Contains(userRole));
    }
    
    public static bool IsAdmin(string userRole)
    {
        return userRole.Equals("Admin");
    }
}