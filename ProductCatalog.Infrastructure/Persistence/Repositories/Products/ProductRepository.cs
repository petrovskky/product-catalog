using ProductCatalog.Application.Interfaces.Repositories.Products;
using ProductCatalog.Domain.Entities;

namespace ProductCatalog.Infrastructure.Persistence.Repositories.Products;

public class ProductRepository(AppDbContext context) : IProductRepository
{
    public async Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await context.Products.FindAsync([id], cancellationToken);
    }

    public async Task AddAsync(Product entity, CancellationToken cancellationToken)
    {
        await context.Products.AddAsync(entity, cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var entity = await context.Products.FindAsync([id], cancellationToken);
        if (entity != null)
            context.Products.Remove(entity);
    }
}