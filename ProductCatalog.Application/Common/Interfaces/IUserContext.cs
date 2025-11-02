namespace ProductCatalog.Application.Common.Interfaces;

public interface IUserContext
{
    Guid? Id { get; }
    string? Name { get; }
    string? Email { get; }
    string? Role { get; }
    bool IsAuthenticated { get; }
}