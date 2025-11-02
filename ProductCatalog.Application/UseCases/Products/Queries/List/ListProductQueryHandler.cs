using MediatR;
using ProductCatalog.Application.Common;
using ProductCatalog.Application.Common.Results;
using ProductCatalog.Application.Common.Services;
using ProductCatalog.Application.Interfaces.Repositories.Products;

namespace ProductCatalog.Application.UseCases.Products.Queries.List;

public class ListProductQueryHandler(IProductReadRepository productRepository) 
    : IRequestHandler<ListProductQuery, Result<PaginatedResult<IEnumerable<ProductDto>>>>
{
    public async Task<Result<PaginatedResult<IEnumerable<ProductDto>>>> Handle(
        ListProductQuery request, CancellationToken cancellationToken)
    {
        var paginatedProducts = await productRepository.GetAllAsync(
            request.ProductFilter, request.SortParams, request.PageParams, cancellationToken);

        var isPrivileged = RolePrivileges.HasPrivilegedAccess(request.UserRole);

        var productsDto = paginatedProducts.Data
            .Select(p => new ProductDto(p.Id, p.Name, p.CategoryId, p.Category.Name, p.Description, 
                p.Price, p.Note, isPrivileged ? p.SpecialNote : null))
            .ToList();

        var result = new PaginatedResult<IEnumerable<ProductDto>>(
            data: productsDto,
            totalCount: paginatedProducts.TotalCount
        );

        return Result<PaginatedResult<IEnumerable<ProductDto>>>.Success(result);
    }
}