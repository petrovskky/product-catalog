using MediatR;
using ProductCatalog.Application.Common.Results;
using ProductCatalog.Application.Common.Services;
using ProductCatalog.Application.Interfaces.Repositories;
using ProductCatalog.Application.Interfaces.Repositories.Products;
using ProductCatalog.Domain.Entities;

namespace ProductCatalog.Application.UseCases.Products.Queries.Get;

public class GetProductQueryHandler(IProductReadRepository productRepository) 
    : IRequestHandler<GetProductQuery, Result<ProductDto>>
{
    public async Task<Result<ProductDto>> Handle(GetProductQuery request, 
        CancellationToken cancellationToken)
    {
        var product = await productRepository.GetByIdAsync(request.Id, cancellationToken);
        
        if (product == null)
            return Result<ProductDto>.Failure("Продукт не найден");
        
        var isPrivileged = RolePrivileges.HasPrivilegedAccess(request.UserRole);

        return new ProductDto(product.Id, product.Name, product.CategoryId, product.Category.Name, 
            product.Description, product.Price, product.Note, isPrivileged ? product.SpecialNote : null);
    }
}