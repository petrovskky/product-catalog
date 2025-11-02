using ProductCatalog.Domain.Entities;

namespace ProductCatalog.Application.Interfaces.Repositories.Users;

public interface IUserReadRepository
{
    Task<bool> IsBlockedByIdAsync(Guid userId, CancellationToken cancellationToken);
    Task<User?> GetByIdAsync(Guid userId, CancellationToken cancellationToken);
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken);
    Task<IEnumerable<User>> GetAllAsync(CancellationToken cancellationToken);
}