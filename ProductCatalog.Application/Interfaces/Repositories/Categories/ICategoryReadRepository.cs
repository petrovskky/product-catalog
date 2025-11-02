using ProductCatalog.Application.UseCases.Categories;
using ProductCatalog.Domain.Entities;

namespace ProductCatalog.Application.Interfaces.Repositories.Categories;

public interface ICategoryReadRepository
{
    Task<Category?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<IEnumerable<Category>> GetAllAsync(CancellationToken cancellationToken);
}