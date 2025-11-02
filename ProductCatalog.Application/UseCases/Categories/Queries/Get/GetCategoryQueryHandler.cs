using MediatR;
using ProductCatalog.Application.Common.Results;
using ProductCatalog.Application.Interfaces.Repositories;
using ProductCatalog.Application.Interfaces.Repositories.Categories;

namespace ProductCatalog.Application.UseCases.Categories.Queries.Get;

public class GetCategoryQueryHandler(ICategoryRepository categoryRepository) 
    : IRequestHandler<GetCategoryQuery, Result<CategoryDto>>
{
    public async Task<Result<CategoryDto>> Handle(GetCategoryQuery request, 
        CancellationToken cancellationToken)
    {
        var category = await categoryRepository.GetByIdAsync(request.Id, cancellationToken);
        
        return category == null
            ? Result<CategoryDto>.Failure("Категория не найдена") 
            : new CategoryDto(category.Id, category.Name); 
    }
}