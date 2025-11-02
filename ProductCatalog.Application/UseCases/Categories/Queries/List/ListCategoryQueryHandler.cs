using MediatR;
using ProductCatalog.Application.Common.Results;
using ProductCatalog.Application.Interfaces.Repositories.Categories;
using ProductCatalog.Application.UseCases.Products;

namespace ProductCatalog.Application.UseCases.Categories.Queries.List;

public class ListCategoryQueryHandler(ICategoryReadRepository categoryRepository) 
    : IRequestHandler<ListCategoryQuery, Result<IEnumerable<CategoryDto>>>
{
    public async Task<Result<IEnumerable<CategoryDto>>> Handle(ListCategoryQuery request, CancellationToken cancellationToken)
    {
        var categories = await categoryRepository.GetAllAsync(cancellationToken);
        
        return categories
            .Select(p => new CategoryDto(p.Id, p.Name))
            .ToList();
    }
}