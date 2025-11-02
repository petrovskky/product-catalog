using Microsoft.EntityFrameworkCore;
using ProductCatalog.Application.Common;
using ProductCatalog.Application.Filters;
using ProductCatalog.Application.Interfaces.Repositories.Products;
using ProductCatalog.Domain.Entities;
using ProductCatalog.Infrastructure.Persistence.Extensions;

namespace ProductCatalog.Infrastructure.Persistence.Repositories.Products;

public class ProductReadRepository(AppDbContext context) : IProductReadRepository
{
    public async Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await context.Products
            .AsNoTracking()
            .Include(p => p.Category)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<PaginatedResult<IEnumerable<Product>>> GetAllAsync(ProductFilter productFilter, 
        SortParams sortParams, PageParams pageParams, CancellationToken cancellationToken)
    {
        return await context.Products
            .AsNoTracking()
            .Include(p => p.Category)
            .Filter(productFilter)
            .Sort(sortParams)
            .ToPaginatedAsync(pageParams);
    }
}