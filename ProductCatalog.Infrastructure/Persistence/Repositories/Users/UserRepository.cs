using Microsoft.EntityFrameworkCore;
using ProductCatalog.Application.Interfaces.Repositories.Users;
using ProductCatalog.Domain.Entities;

namespace ProductCatalog.Infrastructure.Persistence.Repositories.Users;

public class UserRepository(AppDbContext context) : IUserRepository
{
    public async Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        => await context.Users.FindAsync([id], cancellationToken);

    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken)
        => await context.Users.FirstOrDefaultAsync(u => u.Email == email, cancellationToken);

    public async Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken)
        => await context.Users.AnyAsync(u => u.Email == email, cancellationToken);

    public async Task AddAsync(User user, CancellationToken cancellationToken)
        => await context.Users.AddAsync(user, cancellationToken);

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var entity = await context.Users.FindAsync([id], cancellationToken);
        if (entity != null)
            context.Users.Remove(entity);
    }
}