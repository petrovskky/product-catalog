namespace ProductCatalog.Domain.Entities;

public class User
{
    public Guid Id { get; set; }
    public string Email { get; set; }
    public string Name { get; set; }
    public string Role { get; set; }
    public bool IsBlocked { get; set; }
    public string PasswordHash { get; set; }
}
