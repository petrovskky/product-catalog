namespace ProductCatalog.Api.Dtos.Users;

public record CreateUserRequest(string Email, string Name, string Role, string Password);