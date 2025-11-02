using Microsoft.EntityFrameworkCore;
using ProductCatalog.Application.Interfaces.Repositories.Users;
using ProductCatalog.Domain.Entities;

namespace ProductCatalog.Infrastructure.Persistence.Repositories.Users;

public class UserReadRepository(AppDbContext context) : IUserReadRepository
{
    public async Task<bool> IsBlockedByIdAsync(Guid userId, CancellationToken cancellationToken)
    {
        return await context.Users
            .AsNoTracking()
            .Where(u => u.Id == userId)
            .Select(u => u.IsBlocked)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
    }

    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken)
    {
        return await context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
    }


    public async Task<IEnumerable<User>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await context.Users
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }
}