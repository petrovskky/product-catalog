using Microsoft.EntityFrameworkCore;
using ProductCatalog.Application.Interfaces.Repositories.Categories;
using ProductCatalog.Domain.Entities;

namespace ProductCatalog.Infrastructure.Persistence.Repositories.Categories;

public class CategoryReadRepository(AppDbContext context) : ICategoryReadRepository
{
    public async Task<Category?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await context.Categories
            .AsNoTracking()
            .Include(c => c.Products)
            .FirstOrDefaultAsync(i => i.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<Category>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await context.Categories
            .AsNoTracking()
            .Include(c => c.Products)
            .ToListAsync(cancellationToken);
    }
}