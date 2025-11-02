using ProductCatalog.Domain.Entities;

namespace ProductCatalog.Application.Interfaces.Repositories.Categories;

public interface ICategoryRepository
{
    Task<Category?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<bool> ExistsByIdAsync(Guid id, CancellationToken cancellationToken);
    Task AddAsync(Category entity, CancellationToken cancellationToken);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken);
}