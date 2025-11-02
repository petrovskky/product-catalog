namespace ProductCatalog.Application.UseCases.Users;

public record UserDto(Guid Id, string Email, string Name, string Role, bool IsBlocked);