using Microsoft.EntityFrameworkCore;
using ProductCatalog.Application.Interfaces.Repositories.Categories;
using ProductCatalog.Domain.Entities;

namespace ProductCatalog.Infrastructure.Persistence.Repositories.Categories;

public class CategoryRepository(AppDbContext context) : ICategoryRepository
{
    public async Task<Category?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await context.Categories.FindAsync([id], cancellationToken);
    }

    public async Task<bool> ExistsByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await context.Categories.AnyAsync(c => c.Id == id, cancellationToken);
    }

    public async Task AddAsync(Category entity, CancellationToken cancellationToken)
    {
        await context.Categories.AddAsync(entity, cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var entity = await context.Categories.FindAsync([id], cancellationToken);
        if (entity != null)
            context.Categories.Remove(entity);
    }
}