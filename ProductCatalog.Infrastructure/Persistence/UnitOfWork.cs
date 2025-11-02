using ProductCatalog.Application.Interfaces;
using ProductCatalog.Application.Interfaces.Repositories;

namespace ProductCatalog.Infrastructure.Persistence;

public class UnitOfWork(AppDbContext context) : IUnitOfWork
{
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return context.SaveChangesAsync(cancellationToken);
    }
}