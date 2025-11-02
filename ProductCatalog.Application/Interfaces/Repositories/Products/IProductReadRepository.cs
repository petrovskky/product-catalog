using ProductCatalog.Application.Common;
using ProductCatalog.Application.Filters;
using ProductCatalog.Domain.Entities;

namespace ProductCatalog.Application.Interfaces.Repositories.Products;

public interface IProductReadRepository
{
    Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<PaginatedResult<IEnumerable<Product>>> GetAllAsync(ProductFilter productFilter,
        SortParams sortParams, PageParams pageParams, CancellationToken cancellationToken);
}